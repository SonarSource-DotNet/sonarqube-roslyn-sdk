//-----------------------------------------------------------------------
// <copyright file="JarChecker.cs" company="SonarSource SA and Microsoft Corporation">
//   Copyright (c) SonarSource SA and Microsoft Corporation.  All rights reserved.
//   Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
//-----------------------------------------------------------------------
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.IO.Compression;
using System.Linq;
using SonarQube.Plugins.Test.Common;

namespace SonarQube.Plugins.PluginGeneratorTests
{
    internal class JarChecker
    {
        private readonly string unzippedDir;

        public JarChecker(TestContext testContext, string fullJarPath)
        {
            TestUtils.AssertFileExists(fullJarPath);

            this.unzippedDir = TestUtils.CreateTestDirectory(testContext, "unzipped");
            ZipFile.ExtractToDirectory(fullJarPath, this.unzippedDir);
        }

        public void JarContainsFiles(params string[] expectedRelativePaths)
        {
            foreach (string relativePath in expectedRelativePaths)
            {
                string[] matchingFiles = Directory.GetFiles(this.unzippedDir, relativePath, SearchOption.TopDirectoryOnly);

                Assert.IsTrue(matchingFiles.Length < 2, "Test error: supplied relative path should not match multiple files");
                string fullFilePath = matchingFiles.FirstOrDefault();

                Assert.IsTrue(File.Exists(fullFilePath), "Jar does not contain expected file: {0}", relativePath);
            }
        }

    }
}
