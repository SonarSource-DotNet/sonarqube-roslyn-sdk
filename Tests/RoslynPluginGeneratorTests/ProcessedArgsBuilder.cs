﻿/*
 * SonarQube Roslyn SDK
 * Copyright (C) 2015-2018 SonarSource SA
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

using NuGet;
using SonarQube.Plugins.Roslyn.CommandLine;

namespace SonarQube.Plugins.Roslyn.RoslynPluginGeneratorTests
{
    public class ProcessedArgsBuilder
    {
        private string packageId;
        private string packageVersion;
        private string language;
        private string sqaleFilePath;
        private string ruleFilePath;
        private bool acceptLicenses;
        private bool recurseDependencies;
        private string outputDirectory;

        public ProcessedArgsBuilder(string packageId, string outputDir)
        {
            this.packageId = packageId;
            this.outputDirectory = outputDir;
        }
        public ProcessedArgsBuilder SetPackageVersion(string packageVersion)
        {
            this.packageVersion = packageVersion;
            return this;
        }

        public ProcessedArgsBuilder SetLanguage(string language)
        {
            this.language = language;
            return this;
        }

        public ProcessedArgsBuilder SetSqaleFilePath(string filePath)
        {
            this.sqaleFilePath = filePath;
            return this;
        }

        public ProcessedArgsBuilder SetRuleFilePath(string filePath)
        {
            this.ruleFilePath = filePath;
            return this;
        }

        public ProcessedArgsBuilder SetAcceptLicenses(bool acceptLicenses)
        {
            this.acceptLicenses = acceptLicenses;
            return this;
        }

        public ProcessedArgsBuilder SetRecurseDependencies(bool recurseDependencies)
        {
            this.recurseDependencies = recurseDependencies;
            return this;
        }

        public ProcessedArgs Build()
        {
            ProcessedArgs args = new ProcessedArgs(
                packageId,
                new SemanticVersion(packageVersion),
                language,
                sqaleFilePath,
                ruleFilePath,
                acceptLicenses,
                recurseDependencies,
                outputDirectory);
            return args;
        }
    }
}
