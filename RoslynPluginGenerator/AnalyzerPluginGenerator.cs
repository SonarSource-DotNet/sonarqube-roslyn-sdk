﻿/*
 * SonarQube Roslyn SDK
 * Copyright (C) 2015-2025 SonarSource SA
 * mailto:info AT sonarsource DOT com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software Foundation,
 * Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.Diagnostics;
using NuGet;
using SonarQube.Plugins.Common;
using SonarQube.Plugins.Roslyn.CommandLine;

namespace SonarQube.Plugins.Roslyn
{
    public class AnalyzerPluginGenerator
    {
        /// <summary>
        /// The prefix expected by the C# plugin; used to identify repositories
        /// that contain Roslyn rules
        /// </summary>
        private const string RepositoryKeyPrefix = "roslyn.";

        /// <summary>
        /// List of file extensions that should not be included in the zipped analyzer assembly
        /// </summary>
        private static readonly string[] excludedFileExtensions = { ".nupkg", ".nuspec" };

        /// <summary>
        /// Specifies the format for the name of the template rules xml file
        /// </summary>
        public const string RulesTemplateFileNameFormat = "{0}.{1}.rules.template.xml";

        private readonly INuGetPackageHandler packageHandler;
        private readonly SonarQube.Plugins.Common.ILogger logger;

        public AnalyzerPluginGenerator(INuGetPackageHandler packageHandler, SonarQube.Plugins.Common.ILogger logger)
        {
            this.packageHandler = packageHandler ?? throw new ArgumentNullException(nameof(packageHandler));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool Generate(ProcessedArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            IPackage targetPackage = packageHandler.FetchPackage(args.PackageId, args.PackageVersion);

            if (targetPackage == null)
            {
                return false;
            }

            IEnumerable<IPackage> dependencyPackages = packageHandler.GetInstalledDependencies(targetPackage);

            // Check that there are analyzers in the target package from which information can be extracted

            // Create a mapping of packages to analyzers to avoid having to search for analyzers more than once
            Dictionary<IPackage, IEnumerable<DiagnosticAnalyzer>> analyzersByPackage = new Dictionary<IPackage, IEnumerable<DiagnosticAnalyzer>>();
            IEnumerable<DiagnosticAnalyzer> targetAnalyzers = GetAnalyzers(targetPackage, args.Language);
            if (targetAnalyzers.Any())
            {
                analyzersByPackage.Add(targetPackage, targetAnalyzers);
            }
            else
            {
                logger.LogWarning(UIResources.APG_NoAnalyzersFound, targetPackage.Id);

                if (!args.RecurseDependencies)
                {
                    logger.LogWarning(UIResources.APG_NoAnalyzersInTargetSuggestRecurse);
                    logger.LogError(UIResources.APG_PluginNotGenerated);
                    return false;
                }
            }

            if (args.RecurseDependencies)
            {
                // Possible sub-case - target package has dependencies that contain analyzers
                foreach (IPackage dependencyPackage in dependencyPackages)
                {
                    IEnumerable<DiagnosticAnalyzer> dependencyAnalyzers = GetAnalyzers(dependencyPackage, args.Language);
                    if (dependencyAnalyzers.Any())
                    {
                        analyzersByPackage.Add(dependencyPackage, dependencyAnalyzers);
                    }
                    else
                    {
                        logger.LogWarning(UIResources.APG_NoAnalyzersFound, dependencyPackage.Id);
                    }
                }

                if (!analyzersByPackage.Any())
                {
                    logger.LogError(UIResources.APG_PluginNotGenerated);
                    return false;
                }
            }

            // Check for packages that require the user to accept a license
            IEnumerable<IPackage> licenseAcceptancePackages = GetPackagesRequiringLicenseAcceptance(targetPackage);
            if (licenseAcceptancePackages.Any() && !args.AcceptLicenses)
            {
                // NB: This warns for all packages under the target that require license acceptance
                // (even if they aren't related to the packages from which plugins were generated)
                logger.LogError(UIResources.APG_NGPackageRequiresLicenseAcceptance, targetPackage.Id, targetPackage.Version.ToString());
                ListPackagesRequiringLicenseAcceptance(licenseAcceptancePackages);
                return false;
            }

            List<string> generatedJarFiles = new List<string>();
            // Initial run with the user-targeted package and arguments
            if (analyzersByPackage.ContainsKey(targetPackage))
            {
                string generatedJarPath = GeneratePluginForPackage(args.OutputDirectory, args.Language, args.RuleFilePath, targetPackage, analyzersByPackage[targetPackage]);
                if (generatedJarPath == null)
                {
                    return false;
                }

                generatedJarFiles.Add(generatedJarPath);
                analyzersByPackage.Remove(targetPackage);
            }

            // Dependent package generation changes the arguments
            if (args.RecurseDependencies)
            {
                logger.LogWarning(UIResources.APG_RecurseEnabled_RuleCustomizationNotEnabled);

                foreach (IPackage currentPackage in analyzersByPackage.Keys)
                {
                    // No way to specify the rules xml files for any but the user-targeted package at this time
                    string generatedJarPath = GeneratePluginForPackage(args.OutputDirectory, args.Language, null, currentPackage, analyzersByPackage[currentPackage]);
                    if (generatedJarPath == null)
                    {
                        return false;
                    }

                    generatedJarFiles.Add(generatedJarPath);
                }
            }

            LogAcceptedPackageLicenses(licenseAcceptancePackages);

            foreach (string generatedJarFile in generatedJarFiles)
            {
                logger.LogInfo(UIResources.APG_PluginGenerated, generatedJarFile);
            }

            return true;
        }

        private string GeneratePluginForPackage(string outputDir, string language, string rulesFilePath, IPackage package, IEnumerable<DiagnosticAnalyzer> analyzers)
        {
            Debug.Assert(analyzers.Any(), "The method must be called with a populated list of DiagnosticAnalyzers.");

            logger.LogInfo(UIResources.APG_AnalyzersLocated, package.Id, analyzers.Count());

            string createdJarFilePath = null;

            string baseDirectory = CreateBaseWorkingDirectory();

            // Collect the remaining data required to build the plugin
            RoslynPluginDefinition definition = new RoslynPluginDefinition
            {
                Language = language,
                RulesFilePath = rulesFilePath,
                PackageId = package.Id,
                PackageVersion = package.Version.ToString(),
                Manifest = CreatePluginManifest(package)
            };

            // Create a zip containing the required analyzer files
            string packageDir = packageHandler.GetLocalPackageRootDirectory(package);
            definition.SourceZipFilePath = CreateAnalyzerStaticPayloadFile(packageDir, baseDirectory);
            definition.StaticResourceName = Path.GetFileName(definition.SourceZipFilePath);

            bool generate = true;

            string generatedRulesTemplateFile = null;
            if (definition.RulesFilePath == null)
            {
                definition.RulesFilePath = GenerateRulesFile(analyzers, baseDirectory);
                generatedRulesTemplateFile = CalculateRulesTemplateFileName(package, outputDir);
                File.Copy(definition.RulesFilePath, generatedRulesTemplateFile, overwrite: true);
            }
            else
            {
                this.logger.LogInfo(UIResources.APG_UsingSuppliedRulesFile, definition.RulesFilePath);
                generate = IsValidRulesFile(definition.RulesFilePath);
            }

            if (generate)
            {
                createdJarFilePath = BuildPlugin(definition, outputDir);
            }

            LogMessageForGeneratedRules(generatedRulesTemplateFile);

            return createdJarFilePath;
        }

        private void LogMessageForGeneratedRules(string generatedFile)
        {
            if (generatedFile != null)
            {
                // Log a message about the generated rules xml file for every plugin generated
                this.logger.LogInfo(UIResources.APG_TemplateRuleFileGenerated, generatedFile);
            }
        }

        private void LogAcceptedPackageLicenses(IEnumerable<IPackage> licenseAcceptancePackages)
        {
            if (licenseAcceptancePackages.Any())
            {
                // If we got this far then the user must have accepted
                logger.LogWarning(UIResources.APG_NGAcceptedPackageLicenses);
                ListPackagesRequiringLicenseAcceptance(licenseAcceptancePackages);
            }
        }

        private void ListPackagesRequiringLicenseAcceptance(IEnumerable<IPackage> licensedPackages)
        {
            foreach (IPackage package in licensedPackages)
            {
                string license;
                if (package.LicenseUrl == null)
                {
                    license = UIResources.APG_NG_UnspecifiedLicenseUrl;
                }
                else
                {
                    license = package.LicenseUrl.ToString();
                }
                logger.LogWarning(UIResources.APG_NG_PackageAndLicenseUrl, package.Id, package.Version, license);
            }
        }

        /// <summary>
        /// Returns all of the packages from the supplied package and its dependencies that require license acceptance
        /// </summary>
        private IEnumerable<IPackage> GetPackagesRequiringLicenseAcceptance(IPackage package)
        {
            List<IPackage> licensed = new List<IPackage>();
            if (package.RequireLicenseAcceptance)
            {
                licensed.Add(package);
            }

            licensed.AddRange(packageHandler.GetInstalledDependencies(package).Where(d => d.RequireLicenseAcceptance));
            return licensed;
        }

        /// <summary>
        /// Creates a uniquely-named temp directory for this generation run
        /// </summary>
        private string CreateBaseWorkingDirectory()
        {
            string baseDirectory = Utilities.CreateTempDirectory(".gen");
            baseDirectory = Utilities.CreateSubDirectory(baseDirectory, Guid.NewGuid().ToString());
            logger.LogDebug(UIResources.APG_CreatedTempWorkingDir, baseDirectory);
            return baseDirectory;
        }

        /// <summary>
        /// Retrieves the analyzers contained within a given NuGet package corresponding to a given language
        /// </summary>
        private IEnumerable<DiagnosticAnalyzer> GetAnalyzers(IPackage package, string language)
        {
            string packageRootDir = packageHandler.GetLocalPackageRootDirectory(package);
            string additionalSearchFolder = packageHandler.LocalCacheRoot;

            logger.LogInfo(UIResources.APG_LocatingAnalyzers);
            string[] analyzerFiles = Directory.GetFiles(packageRootDir, "*.dll", SearchOption.AllDirectories);

            string roslynLanguageName = SupportedLanguages.RoslynLanguageName(language);
            logger.LogDebug(UIResources.APG_LogAnalyzerLanguage, roslynLanguageName);

            DiagnosticAssemblyScanner diagnosticAssemblyScanner = new DiagnosticAssemblyScanner(logger, additionalSearchFolder);
            IEnumerable<DiagnosticAnalyzer> analyzers = diagnosticAssemblyScanner.InstantiateDiagnostics(roslynLanguageName, analyzerFiles.ToArray());

            return analyzers;
        }

        private string CreateAnalyzerStaticPayloadFile(string packageRootDir, string outputDir)
        {
            string zipFilePath = Path.GetFileName(packageRootDir) + ".zip";
            zipFilePath = Path.Combine(outputDir, zipFilePath);

            ZipExtensions.CreateFromDirectory(packageRootDir, zipFilePath, IncludeFileInZip);

            return zipFilePath;
        }

        private static bool IncludeFileInZip(string filePath)
        {
            return !excludedFileExtensions.Any(e => filePath.EndsWith(e, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Generate a rules file for the specified analyzers
        /// </summary>
        /// <returns>The full path to the generated file</returns>
        private string GenerateRulesFile(IEnumerable<DiagnosticAnalyzer> analyzers, string baseDirectory)
        {
            logger.LogInfo(UIResources.APG_GeneratingRules);

            Debug.Assert(analyzers.Any(), "Expecting at least one analyzer");

            string rulesFilePath = Path.Combine(baseDirectory, "rules.xml");

            RuleGenerator ruleGen = new RuleGenerator(logger);
            Rules rules = ruleGen.GenerateRules(analyzers);

            if (rules != null)
            {
                rules.Save(rulesFilePath, logger);
                logger.LogDebug(UIResources.APG_RulesGeneratedToFile, rules.Count, rulesFilePath);
            }
            else
            {
                Debug.Fail("Not expecting the generated rules to be null");
            }

            return rulesFilePath;
        }

        private static string CalculateRulesTemplateFileName(IPackage package, string directory)
        {
            string filePath = string.Format(System.Globalization.CultureInfo.CurrentCulture,
                RulesTemplateFileNameFormat, package.Id, package.Version);

            filePath = Path.Combine(directory, filePath);
            return filePath;
        }

        /// <summary>
        /// Checks that the supplied rule file has valid content
        /// </summary>
        private bool IsValidRulesFile(string filePath)
        {
            Debug.Assert(!string.IsNullOrWhiteSpace(filePath));
            // Existence is checked when parsing the arguments
            Debug.Assert(File.Exists(filePath), "Expecting the rule file to exist: " + filePath);

            try
            {
                // TODO: consider adding further checks
                Serializer.LoadModel<Rules>(filePath);

            }
            catch (InvalidOperationException) // will be thrown for invalid xml
            {
                this.logger.LogError(UIResources.APG_InvalidRulesFile, filePath);
                return false;
            }
            return true;
        }
        
        public /* for test */ static PluginManifest CreatePluginManifest(IPackage package)
        {
            // The manifest properties supported by SonarQube are documented at
            // http://docs.sonarqube.org/display/DEV/Build+plugin

            PluginManifest pluginDefn = new PluginManifest
            {
                Description = GetValidManifestString(package.Description),
                Developers = GetValidManifestString(ListToString(package.Authors)),

                Homepage = GetValidManifestString(package.ProjectUrl?.ToString()),
                Key = PluginKeyUtilities.GetValidKey(package.Id)
            };

            if (!String.IsNullOrWhiteSpace(package.Title))
            {
                pluginDefn.Name = GetValidManifestString(package.Title);
            }
            else
            {
                // Process the package ID to replace dot separators with spaces for use as a fallback
                pluginDefn.Name = GetValidManifestString(package.Id.Replace(".", " "));
            }

            // Fall back to using the authors if owners is empty
            string organisation;
            if (package.Owners.Any())
            {
                organisation = ListToString(package.Owners);
            }
            else
            {
                organisation = ListToString(package.Authors);
            }
            pluginDefn.Organization = GetValidManifestString(organisation);

            pluginDefn.Version = GetValidManifestString(package.Version?.ToNormalizedString());

            // The TermsConditionsUrl is only displayed in the "Update Center - Available" page
            // i.e. for plugins that are available through the public Update Center.
            // If the property has a value then the link will be displayed with a checkbox
            // for acceptance.
            // It is not used when plugins are directly dropped into the extensions\plugins
            // folder of the SonarQube server.
            pluginDefn.TermsConditionsUrl = GetValidManifestString(package.LicenseUrl?.ToString());

            // Packages from the NuGet website may have friendly short licensenames heuristically assigned, but this requires a downcast
            DataServicePackage dataServicePackage = package as DataServicePackage;
            if (!String.IsNullOrWhiteSpace(dataServicePackage?.LicenseNames))
            {
                pluginDefn.License = GetValidManifestString(dataServicePackage.LicenseNames);
            }
            else
            {
                // Fallback - use a raw URL. Not as nice-looking in the UI, but acceptable.
                pluginDefn.License = pluginDefn.TermsConditionsUrl;
            }

            return pluginDefn;
        }

        private static string GetValidManifestString(string value)
        {
            string valid = value;

            if (valid != null)
            {
                valid = valid.Replace('\r', ' ');
                valid = valid.Replace('\n', ' ');
            }
            return valid;
        }

        private static string ListToString(IEnumerable<string> args)
        {
            if (args == null)
            {
                return null;
            }
            return string.Join(",", args);
        }

        /// <summary>
        /// Builds the plugin and returns the name of the jar that was created
        /// </summary>
        private string BuildPlugin(RoslynPluginDefinition definition, string outputDirectory)
        {
            logger.LogInfo(UIResources.APG_GeneratingPlugin);

            // Make the .jar name match the format [artefactid]-[version].jar
            // i.e. the format expected by Maven
            Directory.CreateDirectory(outputDirectory);
            string fullJarPath = Path.Combine(outputDirectory,
                definition.Manifest.Key + "-plugin-" + definition.Manifest.Version + ".jar");

            string repositoryId = RepositoryKeyUtilities.GetValidKey(definition.PackageId + "." + definition.Language);

            string repoKey = RepositoryKeyPrefix + repositoryId;

            RoslynPluginJarBuilder builder = new RoslynPluginJarBuilder(logger);
            builder.SetLanguage(definition.Language)
                        .SetRepositoryKey(repoKey)
                        .SetRepositoryName(definition.Manifest.Name)
                        .SetRulesFilePath(definition.RulesFilePath)
                        .SetPluginManifestProperties(definition.Manifest)
                        .SetJarFilePath(fullJarPath);

            AddRoslynMetadata(builder, definition, repositoryId);

            string relativeStaticFilePath = "static/" + Path.GetFileName(definition.StaticResourceName);
            builder.AddResourceFile(definition.SourceZipFilePath, relativeStaticFilePath);

            builder.Build();
            return fullJarPath;
        }

        private void AddRoslynMetadata(RoslynPluginJarBuilder builder, RoslynPluginDefinition definition, string repositoryId)
        {
            builder.SetPluginProperty(repositoryId + ".nuget.packageId", definition.PackageId);
            builder.SetPluginProperty(repositoryId + ".nuget.packageVersion", definition.PackageVersion);

            builder.SetPluginProperty(repositoryId + ".analyzerId", definition.PackageId);
            builder.SetPluginProperty(repositoryId + ".ruleNamespace", definition.PackageId);
            builder.SetPluginProperty(repositoryId + ".staticResourceName", definition.StaticResourceName);
            builder.SetPluginProperty(repositoryId + ".pluginKey", definition.Manifest.Key);
            builder.SetPluginProperty(repositoryId + ".pluginVersion", definition.Manifest.Version);
        }
    }
}