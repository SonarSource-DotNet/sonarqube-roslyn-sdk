﻿//-----------------------------------------------------------------------
// <copyright file="AnalyzerPluginGeneratorTests.cs" company="SonarSource SA and Microsoft Corporation">
//   Copyright (c) SonarSource SA and Microsoft Corporation.  All rights reserved.
//   Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NuGet;
using SonarLint.XmlDescriptor;
using SonarQube.Plugins.Roslyn.CommandLine;
using SonarQube.Plugins.Test.Common;
using System;
using System.IO;
using static SonarQube.Plugins.Roslyn.RoslynPluginGeneratorTests.RemoteRepoBuilder;

namespace SonarQube.Plugins.Roslyn.RoslynPluginGeneratorTests
{
    /// <summary>
    /// Tests for NuGetPackageHandler.cs
    /// </summary>
    [TestClass]
    public class AnalyzerPluginGeneratorTests
    {
        public TestContext TestContext { get; set; }

        private enum Node { Root, Child1, Child2, Grandchild1_1, Grandchild2_1, Grandchild2_2 };

        [TestMethod]
        public void Generate_NoAnalyzersFoundInPackage_GenerateFails()
        {
            // Arrange
            string outputDir = TestUtils.CreateTestDirectory(this.TestContext, ".out");

            TestLogger logger = new TestLogger();

            // Create a fake remote repo containing a package that does not contain analyzers
            RemoteRepoBuilder remoteRepoBuilder = new RemoteRepoBuilder(this.TestContext);
            remoteRepoBuilder.CreatePackage("no.analyzers.id", "0.9", TestUtils.CreateTextFile("dummy.txt", outputDir), License.NotRequired /* no dependencies */ );

            NuGetPackageHandler nuGetHandler = new NuGetPackageHandler(remoteRepoBuilder.FakeRemoteRepo, GetLocalNuGetDownloadDir(), logger);
            AnalyzerPluginGenerator apg = new AnalyzerPluginGenerator(nuGetHandler, logger);
            ProcessedArgs args = CreateArgs("no.analyzers.id", "0.9", "cs", null, false, outputDir);

            // Act
            bool result = apg.Generate(args);

            // Assert
            Assert.IsFalse(result, "Expecting generation to fail");
            logger.AssertWarningsLogged(1);
            AssertSqaleTemplateDoesNotExist(outputDir);
        }

        [TestMethod]
        public void Generate_LicenseAcceptanceNotRequired_Succeeds()
        {
            // Arrange
            string outputDir = TestUtils.CreateTestDirectory(this.TestContext, ".out");
            RemoteRepoBuilder remoteRepoBuilder = new RemoteRepoBuilder(this.TestContext);

            // Multi-level dependencies: no package requires license acceptence
            IPackage grandchild = CreatePackageWithAnalyzer(remoteRepoBuilder, "grandchild.id", "1.2", License.NotRequired /* no dependencies */);
            IPackage child = CreatePackageWithAnalyzer(remoteRepoBuilder, "child.id", "1.1", License.NotRequired, grandchild);
            CreatePackageWithAnalyzer(remoteRepoBuilder, "parent.id", "1.0", License.NotRequired, child);

            TestLogger logger = new TestLogger();
            AnalyzerPluginGenerator apg = CreateTestSubjectWithFakeRemoteRepo(remoteRepoBuilder, logger);

            // 1. Acceptance not required -> succeeds if accept = false
            ProcessedArgs args = CreateArgs("parent.id", "1.0", "cs", null, false /* accept licenses */ , outputDir);
            bool result = apg.Generate(args);
            Assert.IsTrue(result, "Generator should succeed if there are no licenses to accept");

            logger.AssertErrorsLogged(0);
            logger.AssertWarningNotLogged("parent.id"); // not expecting warnings about packages that don't require acceptance
            logger.AssertWarningNotLogged("child.id");
            logger.AssertWarningNotLogged("grandchild.id");

            // 2. Acceptance not required -> succeeds if accept = true
            args = CreateArgs("parent.id", "1.0", "cs", null, true /* accept licenses */ , outputDir);
            result = apg.Generate(args);
            Assert.IsTrue(result, "Generator should succeed if there are no licenses to accept");

            logger.AssertErrorsLogged(0);
            logger.AssertWarningNotLogged("parent.id"); // not expecting warnings about packages that don't require acceptance
            logger.AssertWarningNotLogged("child.id");
            logger.AssertWarningNotLogged("grandchild.id");
        }

        [TestMethod]
        public void Generate_LicenseAcceptanceRequiredByMainPackage()
        {
            // Arrange
            string outputDir = TestUtils.CreateTestDirectory(this.TestContext, ".out");
            RemoteRepoBuilder remoteRepoBuilder = new RemoteRepoBuilder(this.TestContext);

            // Parent and child: only parent requires license
            IPackage child = CreatePackageWithAnalyzer(remoteRepoBuilder, "child.id", "1.1", License.NotRequired);
            CreatePackageWithAnalyzer(remoteRepoBuilder, "parent.requiredAccept.id", "1.0", License.Required, child);

            TestLogger logger = new TestLogger();
            AnalyzerPluginGenerator apg = CreateTestSubjectWithFakeRemoteRepo(remoteRepoBuilder, logger);

            // 1. User does not accept -> fails with error
            ProcessedArgs args = CreateArgs("parent.requiredAccept.id", "1.0", "cs", null, false /* accept licenses */ , outputDir);
            bool result = apg.Generate(args);
            Assert.IsFalse(result, "Generator should fail because license has not been accepted");

            logger.AssertSingleErrorExists("parent.requiredAccept.id", "1.0"); // error listing the main package
            logger.AssertSingleWarningExists("parent.requiredAccept.id", "1.0"); // warning for each licensed package
            logger.AssertWarningsLogged(1);

            // 2. User accepts -> succeeds with warnings
            logger.Reset();
            args = CreateArgs("parent.requiredAccept.id", "1.0", "cs", null, true /* accept licenses */ , outputDir);
            result = apg.Generate(args);
            Assert.IsTrue(result, "Generator should succeed if licenses are accepted");

            logger.AssertSingleWarningExists(UIResources.APG_NGAcceptedPackageLicenses); // warning that licenses accepted
            logger.AssertSingleWarningExists("parent.requiredAccept.id", "1.0"); // warning for each licensed package
            logger.AssertWarningsLogged(2);
            logger.AssertErrorsLogged(0);
        }

        [TestMethod]
        public void Generate_LicenseAcceptanceRequiredByDependency()
        {
            // Arrange
            string outputDir = TestUtils.CreateTestDirectory(this.TestContext, ".out");
            RemoteRepoBuilder remoteRepoBuilder = new RemoteRepoBuilder(this.TestContext);

            // Parent and child: only child requires license
            IPackage child = CreatePackageWithAnalyzer(remoteRepoBuilder, "child.requiredAccept.id", "2.0", License.Required);
            CreatePackageWithAnalyzer(remoteRepoBuilder, "parent.id", "1.0", License.NotRequired, child);

            TestLogger logger = new TestLogger();
            AnalyzerPluginGenerator apg = CreateTestSubjectWithFakeRemoteRepo(remoteRepoBuilder, logger);

            // 1. User does not accept -> fails with error
            ProcessedArgs args = CreateArgs("parent.id", "1.0", "cs", null, false /* accept licenses */ , outputDir);
            bool result = apg.Generate(args);
            Assert.IsFalse(result, "Generator should fail because license has not been accepted");

            logger.AssertSingleErrorExists("parent.id", "1.0"); // error listing the main package
            logger.AssertSingleWarningExists("child.requiredAccept.id", "2.0"); // warning for each licensed package
            logger.AssertWarningsLogged(1);

            // 2. User accepts -> succeeds with warnings
            logger.Reset();
            args = CreateArgs("parent.id", "1.0", "cs", null, true /* accept licenses */ , outputDir);
            result = apg.Generate(args);
            Assert.IsTrue(result, "Generator should succeed if licenses are accepted");

            logger.AssertSingleWarningExists(UIResources.APG_NGAcceptedPackageLicenses); // warning that licenses have been accepted
            logger.AssertSingleWarningExists("child.requiredAccept.id", "2.0"); // warning for each licensed package
            logger.AssertWarningsLogged(2);
            logger.AssertErrorsLogged(0);
        }

        [TestMethod]
        public void Generate_LicenseAcceptanceRequired_ByParentAndDependencies()
        {
            // Arrange
            string outputDir = TestUtils.CreateTestDirectory(this.TestContext, ".out");
            RemoteRepoBuilder remoteRepoBuilder = new RemoteRepoBuilder(this.TestContext);

            // Multi-level: parent and some but not all dependencies require license acceptance
            IPackage grandchild1 = CreatePackageWithAnalyzer(remoteRepoBuilder, "grandchild1.requiredAccept.id", "3.0", License.Required);
            IPackage child1 = CreatePackageWithAnalyzer(remoteRepoBuilder, "child1.requiredAccept.id", "2.1", License.Required);
            IPackage child2 = CreatePackageWithAnalyzer(remoteRepoBuilder, "child2.id", "2.2", License.NotRequired, grandchild1);
            CreatePackageWithAnalyzer(remoteRepoBuilder, "parent.requiredAccept.id", "1.0", License.Required, child1, child2);

            TestLogger logger = new TestLogger();
            AnalyzerPluginGenerator apg = CreateTestSubjectWithFakeRemoteRepo(remoteRepoBuilder, logger);

            // 1. User does not accept -> fails with error
            ProcessedArgs args = CreateArgs("parent.requiredAccept.id", "1.0", "cs", null, false /* accept licenses */ , outputDir);
            bool result = apg.Generate(args);
            Assert.IsFalse(result, "Generator should fail because license has not been accepted");

            logger.AssertSingleErrorExists("parent.requiredAccept.id", "1.0"); // error referring to the main package

            logger.AssertSingleWarningExists("grandchild1.requiredAccept.id", "3.0"); // warning for each licensed package
            logger.AssertSingleWarningExists("child1.requiredAccept.id", "2.1");
            logger.AssertSingleWarningExists("parent.requiredAccept.id", "1.0");

            // 2. User accepts -> succeeds with warnings
            logger.Reset();
            args = CreateArgs("parent.requiredAccept.id", "1.0", "cs", null, true /* accept licenses */ , outputDir);
            result = apg.Generate(args);
            Assert.IsTrue(result, "Generator should succeed if licenses are accepted");

            logger.AssertSingleWarningExists(UIResources.APG_NGAcceptedPackageLicenses); // warning that licenses have been accepted
            logger.AssertSingleWarningExists("grandchild1.requiredAccept.id", "3.0"); // warning for each licensed package
            logger.AssertSingleWarningExists("child1.requiredAccept.id", "2.1");
            logger.AssertSingleWarningExists("parent.requiredAccept.id", "1.0");
            logger.AssertWarningsLogged(4);
        }

        [TestMethod]
        public void Generate_LicenseAcceptanceNotRequestedIfNoAnalysers()
        {
            // No point in asking the user to accept licenses for packages that don't contain analyzers

            // Arrange
            string outputDir = TestUtils.CreateTestDirectory(this.TestContext, ".out");
            string dummyContentFile = TestUtils.CreateTextFile("dummy.txt", outputDir, "non-analyzer content file");

            RemoteRepoBuilder remoteRepoBuilder = new RemoteRepoBuilder(this.TestContext);

            // Parent only: requires license
            remoteRepoBuilder.CreatePackage("non.analyzer.requireAccept.id", "1.0", dummyContentFile, License.Required);

            TestLogger logger = new TestLogger();
            AnalyzerPluginGenerator apg = CreateTestSubjectWithFakeRemoteRepo(remoteRepoBuilder, logger);

            // 1. User does not accept, but no analyzers so no license prompt -> fails due absence of analyzers
            ProcessedArgs args = CreateArgs("non.analyzer.requireAccept.id", "1.0", "cs", null, false /* accept licenses */ , outputDir);
            bool result = apg.Generate(args);
            Assert.IsFalse(result, "Expecting generator to fail");

            logger.AssertSingleWarningExists(UIResources.APG_NoAnalyzersFound);
            logger.AssertWarningsLogged(1);
            logger.AssertErrorsLogged(0);
        }

        [TestMethod]
        public void Generate_SqaleFileNotSpecified_TemplateFileCreated()
        {
            // Arrange
            string outputDir = TestUtils.CreateTestDirectory(this.TestContext, ".out");

            TestLogger logger = new TestLogger();

            RemoteRepoBuilder remoteRepoBuilder = new RemoteRepoBuilder(this.TestContext);
            CreatePackageInFakeRemoteRepo(remoteRepoBuilder, "dummy.id", "1.1");

            NuGetPackageHandler nuGetHandler = new NuGetPackageHandler(remoteRepoBuilder.FakeRemoteRepo, GetLocalNuGetDownloadDir(), logger);

            string expectedTemplateSqaleFilePath = Path.Combine(outputDir, "dummy.id.1.1.sqale.template.xml");

            AnalyzerPluginGenerator apg = new AnalyzerPluginGenerator(nuGetHandler, logger);

            ProcessedArgs args = CreateArgs("dummy.id", "1.1", "cs", null, false, outputDir);

            // Act
            bool result = apg.Generate(args);

            // Assert
            Assert.IsTrue(result, "Expecting generation to have succeeded");
            Assert.IsTrue(File.Exists(expectedTemplateSqaleFilePath), "Expecting a template sqale file to have been created");
            this.TestContext.AddResultFile(expectedTemplateSqaleFilePath);
            logger.AssertSingleInfoMessageExists(expectedTemplateSqaleFilePath); // should be a message about the generated file
        }

        [TestMethod]
        public void Generate_ValidSqaleFileSpecified_TemplateFileNotCreated()
        {
            // Arrange
            string outputDir = TestUtils.CreateTestDirectory(this.TestContext, ".out");

            RemoteRepoBuilder remoteRepoBuilder = new RemoteRepoBuilder(this.TestContext);
            AnalyzerPluginGenerator apg = CreateTestSubjectWithFakeRemoteRepo(remoteRepoBuilder);

            CreatePackageInFakeRemoteRepo(remoteRepoBuilder, "dummy.id", "1.1");

            // Create a dummy sqale file
            string dummySqaleFilePath = Path.Combine(outputDir, "inputSqale.xml");
            SqaleModel dummySqale = new SqaleModel();
            Serializer.SaveModel(dummySqale, dummySqaleFilePath);

            ProcessedArgs args = CreateArgs("dummy.id", "1.1", "cs", dummySqaleFilePath, false, outputDir);

            // Act
            bool result = apg.Generate(args);

            // Assert
            Assert.IsTrue(result, "Expecting generation to have succeeded");
            AssertSqaleTemplateDoesNotExist(outputDir);
        }

        [TestMethod]
        public void Generate_InvalidSqaleFileSpecified_GeneratorError()
        {
            // Arrange
            string outputDir = TestUtils.CreateTestDirectory(this.TestContext, ".out");

            TestLogger logger = new TestLogger();

            RemoteRepoBuilder remoteRepoBuilder = new RemoteRepoBuilder(this.TestContext);
            CreatePackageInFakeRemoteRepo(remoteRepoBuilder, "dummy.id", "1.1");

            NuGetPackageHandler nuGetHandler = new NuGetPackageHandler(remoteRepoBuilder.FakeRemoteRepo, GetLocalNuGetDownloadDir(), logger);

            // Create an invalid sqale file
            string dummySqaleFilePath = Path.Combine(outputDir, "invalidSqale.xml");
            File.WriteAllText(dummySqaleFilePath, "not valid xml");

            AnalyzerPluginGenerator apg = new AnalyzerPluginGenerator(nuGetHandler, logger);

            ProcessedArgs args = CreateArgs("dummy.id", "1.1", "cs", dummySqaleFilePath, false, outputDir);

            // Act
            bool result = apg.Generate(args);

            // Assert
            Assert.IsFalse(result, "Expecting generation to have failed");
            AssertSqaleTemplateDoesNotExist(outputDir);
            logger.AssertSingleErrorExists("invalidSqale.xml"); // expecting an error containing the invalid sqale file name
        }

        [TestMethod]
        public void CreatePluginManifest_AllProperties()
        {
            // All properties present, all properties expected.

            // Arrange
            DataServicePackage testPackage = CreateTestDataServicePackage();

            testPackage.Description = "Test Description";
            testPackage.Authors = "TestAuthor1,TestAuthor2";
            testPackage.ProjectUrl = new Uri("http://test.project.url");
            testPackage.Id = "Test.Id";

            testPackage.Title = "Test Title";
            testPackage.Owners = "TestOwner1,TestOwner2";
            testPackage.Version = "1.1.1-RC5";

            testPackage.LicenseUrl = new Uri("http://test.license.url");
            testPackage.LicenseNames = "Test License1;Test License2";

            // Act
            PluginManifest actualPluginManifest = AnalyzerPluginGenerator.CreatePluginManifest(testPackage);

            // Assert
            Assert.IsNotNull(actualPluginManifest);

            Assert.AreEqual(testPackage.Description, actualPluginManifest.Description);
            Assert.AreEqual(testPackage.Authors, actualPluginManifest.Developers);
            Assert.AreEqual(testPackage.ProjectUrl.ToString(), actualPluginManifest.Homepage, false);
            Assert.AreEqual(PluginKeyUtilities.GetValidKey(testPackage.Id), actualPluginManifest.Key);

            Assert.AreEqual(testPackage.Title, actualPluginManifest.Name);
            Assert.AreEqual(testPackage.Owners, actualPluginManifest.Organization);
            Assert.AreEqual(testPackage.Version, actualPluginManifest.Version);

            Assert.AreEqual(testPackage.LicenseUrl.ToString(), actualPluginManifest.TermsConditionsUrl, false);
            Assert.AreEqual(testPackage.LicenseNames, actualPluginManifest.License);
        }

        [TestMethod]
        public void CreatePluginManifest_TitleMissing()
        {
            // When no title is available, ID should be used as a fallback, removing the dot separators for legibility.

            // Arrange
            DataServicePackage testPackage = CreateTestDataServicePackage();
            testPackage.Title = null;
            testPackage.Id = "Foo.Bar.Test";

            // Act
            PluginManifest actualPluginManifest = AnalyzerPluginGenerator.CreatePluginManifest(testPackage);

            // Assert
            Assert.IsNotNull(actualPluginManifest);
            Assert.IsTrue(string.Equals("Foo Bar Test", actualPluginManifest.Name));
        }

        [TestMethod]
        public void CreatePluginManifest_FriendlyLicenseName_Available()
        {
            // When available, a short licensename assigned by NuGet.org should be used.
            // The license url is a fallback only.

            // Arrange
            DataServicePackage testPackage = CreateTestDataServicePackage();
            testPackage.LicenseNames = "Foo Bar License";
            testPackage.LicenseUrl = new System.Uri("http://foo.bar");

            // Act
            PluginManifest actualPluginManifest = AnalyzerPluginGenerator.CreatePluginManifest(testPackage);

            // Assert
            Assert.IsNotNull(actualPluginManifest);
            Assert.AreEqual(testPackage.LicenseNames, actualPluginManifest.License);
        }

        [TestMethod]
        public void CreatePluginManifest_FriendlyLicenseName_NotAvailable()
        {
            // When a short licensename is not assigned by NuGet.org, we should try to use the license URL instead.

            // Arrange
            DataServicePackage testPackage = CreateTestDataServicePackage();
            testPackage.LicenseNames = null;
            testPackage.LicenseUrl = new System.Uri("http://foo.bar");

            // Act
            PluginManifest actualPluginManifest = AnalyzerPluginGenerator.CreatePluginManifest(testPackage);

            // Assert
            Assert.IsNotNull(actualPluginManifest);
            Assert.AreEqual(testPackage.LicenseUrl.ToString(), actualPluginManifest.License, false);
        }

        [TestMethod]
        public void CreatePluginManifest_FromLocalPackage()
        {
            // We should also be able to create a plugin manifest from an IPackage that is not a DataServicePackage

            // Arrange
            string outputDir = TestUtils.CreateTestDirectory(this.TestContext, ".out");

            RemoteRepoBuilder remoteRepoBuilder = new RemoteRepoBuilder(this.TestContext);
            IPackage testPackage = remoteRepoBuilder.CreatePackage("Foo.Bar", "1.0.0", TestUtils.CreateTextFile("dummy.txt", outputDir), License.NotRequired);

            // Act
            PluginManifest actualPluginManifest = AnalyzerPluginGenerator.CreatePluginManifest(testPackage);

            // Assert
            Assert.IsNotNull(actualPluginManifest);
            Assert.AreEqual(testPackage.LicenseUrl.ToString(), actualPluginManifest.License, false);
        }

        [TestMethod]
        public void CreatePluginManifest_Owners_NotAvailable()
        {
            // When the package.Owners field is null, we should fallback to Authors for setting the organisation.

            // Arrange
            DataServicePackage testPackage = CreateTestDataServicePackage();
            testPackage.Owners = null;
            testPackage.Authors = "Foo,Bar,Test";

            // Act
            PluginManifest actualPluginManifest = AnalyzerPluginGenerator.CreatePluginManifest(testPackage);

            // Assert
            Assert.IsNotNull(actualPluginManifest);
            Assert.AreEqual(testPackage.Authors, actualPluginManifest.Organization);
        }

        #region Private methods

        private static ProcessedArgs CreateArgs(string packageId, string packageVersion, string language, string sqaleFilePath, bool acceptLicenses, string outputDirectory)
        {
            ProcessedArgs args = new ProcessedArgs(
                packageId,
                new SemanticVersion(packageVersion),
                language,
                sqaleFilePath,
                acceptLicenses,
                outputDirectory);
            return args;
        }

        private AnalyzerPluginGenerator CreateTestSubjectWithFakeRemoteRepo(RemoteRepoBuilder remoteRepoBuilder)
        {
            return CreateTestSubjectWithFakeRemoteRepo(remoteRepoBuilder, new TestLogger());
        }

        private AnalyzerPluginGenerator CreateTestSubjectWithFakeRemoteRepo(RemoteRepoBuilder remoteRepoBuilder, TestLogger logger)
        {
            NuGetPackageHandler nuGetHandler = new NuGetPackageHandler(remoteRepoBuilder.FakeRemoteRepo, GetLocalNuGetDownloadDir(), logger);
            return new AnalyzerPluginGenerator(nuGetHandler, logger);
        }

        private static IPackage CreatePackageWithAnalyzer(RemoteRepoBuilder remoteRepoBuilder, string packageId, string packageVersion, License acceptanceRequired, params IPackage[] dependencies)
        {
            return remoteRepoBuilder.CreatePackage(packageId, packageVersion, typeof(RoslynAnalyzer11.CSharpAnalyzer).Assembly.Location, acceptanceRequired, dependencies);
        }

        private void CreatePackageInFakeRemoteRepo(RemoteRepoBuilder remoteRepoBuilder, string packageId, string packageVersion)
        {
            remoteRepoBuilder.CreatePackage(packageId, packageVersion, typeof(RoslynAnalyzer11.AbstractAnalyzer).Assembly.Location, License.NotRequired /* no dependencies */ );
        }

        /// <summary>
        /// Creates a blank DataServicePackage with the required field Id already filled in.
        /// </summary>
        private DataServicePackage CreateTestDataServicePackage()
        {
            DataServicePackage newDataServicePackage = new DataServicePackage()
            {
                Id = "Foo.Bar"
            };
            return newDataServicePackage;
        }

        private string GetLocalNuGetDownloadDir()
        {
            return TestUtils.EnsureTestDirectoryExists(this.TestContext, ".localNuGetDownload");
        }

        private static void AssertSqaleTemplateDoesNotExist(string outputDir)
        {
            string[] matches = Directory.GetFiles(outputDir, "*sqale*template*", SearchOption.AllDirectories);
            Assert.AreEqual(0, matches.Length, "Not expecting any squale template files to exist");
        }

        #endregion

    }
}
