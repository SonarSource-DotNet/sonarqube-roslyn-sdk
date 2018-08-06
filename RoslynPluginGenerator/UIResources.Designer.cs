﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SonarQube.Plugins.Roslyn {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class UIResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal UIResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("SonarQube.Plugins.Roslyn.UIResources", typeof(UIResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Number of analyzers located in package {0}: {1}.
        /// </summary>
        public static string APG_AnalyzersLocated {
            get {
                return ResourceManager.GetString("APG_AnalyzersLocated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Temporary working folder for this run: {0}.
        /// </summary>
        public static string APG_CreatedTempWorkingDir {
            get {
                return ResourceManager.GetString("APG_CreatedTempWorkingDir", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generating fixed remediation cost SQALE file....
        /// </summary>
        public static string APG_GeneratingConstantSqaleFile {
            get {
                return ResourceManager.GetString("APG_GeneratingConstantSqaleFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generating plugin....
        /// </summary>
        public static string APG_GeneratingPlugin {
            get {
                return ResourceManager.GetString("APG_GeneratingPlugin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Generating rules....
        /// </summary>
        public static string APG_GeneratingRules {
            get {
                return ResourceManager.GetString("APG_GeneratingRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified rules xml file is invalid: {0}.
        /// </summary>
        public static string APG_InvalidRulesFile {
            get {
                return ResourceManager.GetString("APG_InvalidRulesFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The specified SQALE file is invalid: {0}.
        /// </summary>
        public static string APG_InvalidSqaleFile {
            get {
                return ResourceManager.GetString("APG_InvalidSqaleFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Looking for analyzers in the package....
        /// </summary>
        public static string APG_LocatingAnalyzers {
            get {
                return ResourceManager.GetString("APG_LocatingAnalyzers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Looking for {0} analyzers.
        /// </summary>
        public static string APG_LogAnalyzerLanguage {
            get {
                return ResourceManager.GetString("APG_LogAnalyzerLanguage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} v{1}: {2}.
        /// </summary>
        public static string APG_NG_PackageAndLicenseUrl {
            get {
                return ResourceManager.GetString("APG_NG_PackageAndLicenseUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {license URL not specified}.
        /// </summary>
        public static string APG_NG_UnspecifiedLicenseUrl {
            get {
                return ResourceManager.GetString("APG_NG_UnspecifiedLicenseUrl", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have accepted the licenses for the following packages:.
        /// </summary>
        public static string APG_NGAcceptedPackageLicenses {
            get {
                return ResourceManager.GetString("APG_NGAcceptedPackageLicenses", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Cannot create a plugin for package {0} v{1} because the packages listed below require licenses to be accepted. Please check the licenses and then re-run the generator specifying the &quot;/acceptLicenses&quot; argument..
        /// </summary>
        public static string APG_NGPackageRequiresLicenseAcceptance {
            get {
                return ResourceManager.GetString("APG_NGPackageRequiresLicenseAcceptance", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No analyzers were found in package: {0}.
        /// </summary>
        public static string APG_NoAnalyzersFound {
            get {
                return ResourceManager.GetString("APG_NoAnalyzersFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Re-run this generator with /recurse if plugins should be generated for the dependencies of this package..
        /// </summary>
        public static string APG_NoAnalyzersInTargetSuggestRecurse {
            get {
                return ResourceManager.GetString("APG_NoAnalyzersInTargetSuggestRecurse", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Plugin generated: {0}.
        /// </summary>
        public static string APG_PluginGenerated {
            get {
                return ResourceManager.GetString("APG_PluginGenerated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Customized rule xml information cannot currently be embedded into plugins generated from package dependencies..
        /// </summary>
        public static string APG_RecurseEnabled_RuleCustomizationNotEnabled {
            get {
                return ResourceManager.GetString("APG_RecurseEnabled_RuleCustomizationNotEnabled", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} rules generated to {1}.
        /// </summary>
        public static string APG_RulesGeneratedToFile {
            get {
                return ResourceManager.GetString("APG_RulesGeneratedToFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to SQALE generated to file {0}.
        /// </summary>
        public static string APG_SqaleGeneratedToFile {
            get {
                return ResourceManager.GetString("APG_SqaleGeneratedToFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///Rules definitions: a template rules xml file for the analyzer was saved to {0}. To customise the rules definitions for the analyzer:
        /// * rename the file
        /// * edit the rules definitions in the file
        /// * re-run this generator specifying the rules xml file to use with the /rules:[filename] argument.
        ///.
        /// </summary>
        public static string APG_TemplateRuleFileGenerated {
            get {
                return ResourceManager.GetString("APG_TemplateRuleFileGenerated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 
        ///SQALE: an empty SQALE file for the analyzer was saved to {0}. To provide SQALE remediation information for the analyzer:
        /// * rename the file
        /// * fill in the appropriate remediation information for each diagnostic
        /// * re-run this generator specifying the sqale file to use with the /sqale:[filename] argument.
        ///.
        /// </summary>
        public static string APG_TemplateSqaleFileGenerated {
            get {
                return ResourceManager.GetString("APG_TemplateSqaleFileGenerated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The language &apos;{0}&apos; is not supported. Valid options are &apos;cs&apos; or &apos;vb&apos;..
        /// </summary>
        public static string APG_UnsupportedLanguage {
            get {
                return ResourceManager.GetString("APG_UnsupportedLanguage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Using the supplied rules xml file: {0}.
        /// </summary>
        public static string APG_UsingSuppliedRulesFile {
            get {
                return ResourceManager.GetString("APG_UsingSuppliedRulesFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Using the SQALE file: {0}.
        /// </summary>
        public static string APG_UsingSuppliedSqaleFile {
            get {
                return ResourceManager.GetString("APG_UsingSuppliedSqaleFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Roslyn Analyzer Plugin Generator for SonarQube.
        /// </summary>
        public static string AssemblyDescription {
            get {
                return ResourceManager.GetString("AssemblyDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The file path for the jar file to be created must be specified.
        /// </summary>
        public static string Builder_Error_OutputJarPathMustBeSpecified {
            get {
                return ResourceManager.GetString("Builder_Error_OutputJarPathMustBeSpecified", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Required plugin definition property is missing: {0}.
        /// </summary>
        public static string Builder_Error_RequiredPropertyMissing {
            get {
                return ResourceManager.GetString("Builder_Error_RequiredPropertyMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The existing jar file will be overwritten.
        /// </summary>
        public static string Builder_ExistingJarWillBeOvewritten {
            get {
                return ResourceManager.GetString("Builder_ExistingJarWillBeOvewritten", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to   Adding new dependency: {0} version {1}.
        /// </summary>
        public static string NG_AddingNewDependency {
            get {
                return ResourceManager.GetString("NG_AddingNewDependency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to   Duplicate dependency: {0} version {1}.
        /// </summary>
        public static string NG_DuplicateDependency {
            get {
                return ResourceManager.GetString("NG_DuplicateDependency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to install package: {0}
        ///Check that there are released versions of the package, or specify a pre-release version identifier..
        /// </summary>
        public static string NG_ERROR_PackageInstallFail {
            get {
                return ResourceManager.GetString("NG_ERROR_PackageInstallFail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No packages with the specified id were found: {0}.
        /// </summary>
        public static string NG_ERROR_PackageNotFound {
            get {
                return ResourceManager.GetString("NG_ERROR_PackageNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Package version was not found: {0}.
        /// </summary>
        public static string NG_ERROR_PackageVersionNotFound {
            get {
                return ResourceManager.GetString("NG_ERROR_PackageVersionNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Failed to resolve dependency: {0} version {1}.
        /// </summary>
        public static string NG_FailedToResolveDependency {
            get {
                return ResourceManager.GetString("NG_FailedToResolveDependency", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fetching NuGet config files....
        /// </summary>
        public static string NG_FetchingConfigFiles {
            get {
                return ResourceManager.GetString("NG_FetchingConfigFiles", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to NuGet file conflict occurred: {0}
        ///The conflict will be ignored..
        /// </summary>
        public static string NG_FileConflictOccurred {
            get {
                return ResourceManager.GetString("NG_FileConflictOccurred", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to - latest version.
        /// </summary>
        public static string NG_IsLatestPackageVersionSuffix {
            get {
                return ResourceManager.GetString("NG_IsLatestPackageVersionSuffix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to     {0}, machine-wide: {1}.
        /// </summary>
        public static string NG_ListEnabledPackageSource {
            get {
                return ResourceManager.GetString("NG_ListEnabledPackageSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Enabled package sources:.
        /// </summary>
        public static string NG_ListingEnablePackageSources {
            get {
                return ResourceManager.GetString("NG_ListingEnablePackageSources", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Attempting to locate package with id &apos;{0}&apos;.
        /// </summary>
        public static string NG_LocatingPackages {
            get {
                return ResourceManager.GetString("NG_LocatingPackages", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No enabled package sources. Please update the NuGet config to specify at least one valid package source..
        /// </summary>
        public static string NG_NoEnabledPackageSources {
            get {
                return ResourceManager.GetString("NG_NoEnabledPackageSources", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Number of packages located: {0}.
        /// </summary>
        public static string NG_NumberOfPackagesLocated {
            get {
                return ResourceManager.GetString("NG_NumberOfPackagesLocated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Package versions:.
        /// </summary>
        public static string NG_PackageVersionListHeader {
            get {
                return ResourceManager.GetString("NG_PackageVersionListHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Resolving dependencies for {0} version {1}....
        /// </summary>
        public static string NG_ResolvingPackageDependencies {
            get {
                return ResourceManager.GetString("NG_ResolvingPackageDependencies", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Version was not specified. Using version {0}..
        /// </summary>
        public static string NG_SelectedPackageVersion {
            get {
                return ResourceManager.GetString("NG_SelectedPackageVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Using version marked as latest..
        /// </summary>
        public static string NG_UsingLatestPackageVersion {
            get {
                return ResourceManager.GetString("NG_UsingLatestPackageVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Skipping rule in analyzer {0}: Duplicate key..
        /// </summary>
        public static string RuleGen_DuplicateKey {
            get {
                return ResourceManager.GetString("RuleGen_DuplicateKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Skipping rule in analyzer {0}: No key found..
        /// </summary>
        public static string RuleGen_EmptyKey {
            get {
                return ResourceManager.GetString("RuleGen_EmptyKey", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to For more information visit &lt;a href=&quot;{0}&quot; target=&quot;_blank&quot;&gt;the rule&apos;s help page&lt;/a&gt;..
        /// </summary>
        public static string RuleGen_ForMoreDetailsLink {
            get {
                return ResourceManager.GetString("RuleGen_ForMoreDetailsLink", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to More Details.
        /// </summary>
        public static string RuleGen_MoreDetailsTitle {
            get {
                return ResourceManager.GetString("RuleGen_MoreDetailsTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No description was provided..
        /// </summary>
        public static string RuleGen_NoDescription {
            get {
                return ResourceManager.GetString("RuleGen_NoDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Could not instantiate analyzers from &apos;{0}&apos;. 
        ///
        ///Error: {1}.
        /// </summary>
        public static string Scanner_AnalyzerInstantiationFail {
            get {
                return ResourceManager.GetString("Scanner_AnalyzerInstantiationFail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loaded analyzer: {0}.
        /// </summary>
        public static string Scanner_AnalyzerLoaded {
            get {
                return ResourceManager.GetString("Scanner_AnalyzerLoaded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loaded: {0} analyzers..
        /// </summary>
        public static string Scanner_AnalyzersLoadSuccess {
            get {
                return ResourceManager.GetString("Scanner_AnalyzersLoadSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Loaded assembly: {0}.
        /// </summary>
        public static string Scanner_AssemblyLoadSuccess {
            get {
                return ResourceManager.GetString("Scanner_AssemblyLoadSuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No analyzers found in assembly {0}.
        /// </summary>
        public static string Scanner_NoAnalyzers {
            get {
                return ResourceManager.GetString("Scanner_NoAnalyzers", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Creating new entry: &apos;{0}&apos;.
        /// </summary>
        public static string ZIP_CreatingNewEntry {
            get {
                return ResourceManager.GetString("ZIP_CreatingNewEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deleting existing entry: &apos;{0}&apos;.
        /// </summary>
        public static string ZIP_DeleteExistingEntry {
            get {
                return ResourceManager.GetString("ZIP_DeleteExistingEntry", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Deleting existing jar....
        /// </summary>
        public static string ZIP_DeletingExistingJar {
            get {
                return ResourceManager.GetString("ZIP_DeletingExistingJar", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Inserting file &apos;{0}&apos;. Source: &apos;{1}&apos;.
        /// </summary>
        public static string ZIP_InsertingFile {
            get {
                return ResourceManager.GetString("ZIP_InsertingFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Jar updated.
        /// </summary>
        public static string ZIP_JarUpdated {
            get {
                return ResourceManager.GetString("ZIP_JarUpdated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Updating jar &apos;{0}&apos;.
        /// </summary>
        public static string ZIP_UpdatingJar {
            get {
                return ResourceManager.GetString("ZIP_UpdatingJar", resourceCulture);
            }
        }
    }
}
