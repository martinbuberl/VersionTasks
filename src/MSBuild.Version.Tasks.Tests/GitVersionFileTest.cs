﻿using System;
using System.IO;
using Ionic.Zip;
using NUnit.Framework;

namespace MSBuild.Version.Tasks.Tests
{
    [TestFixture]
    public class GitVersionFileTest
    {
        [SetUp]
        public void SetUp()
        {
            // extract the repositories in the 'Git.zip' file into the 'temp' folder
            using (ZipFile zipFile = ZipFile.Read(Path.Combine(RepositoriesDirectory, "Git.zip")))
            {
                foreach (ZipEntry zipEntry in zipFile)
                {
                    zipEntry.Extract(TempDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            // in combination with TortoiseGit "Directory.Delete()' throws an "System.UnauthorizedAccessException'
            VersionInfoBase versionInfoBase = new GitVersionFile();
            versionInfoBase.WorkingDirectory = TempDirectory;
            versionInfoBase.ExecuteCommand("rm", "-rf *");

            // delete the 'temp' folder
            Directory.Delete(TempDirectory, true);
        }

        [Test]
        public void Execute_WriteChangeset_ShouldWriteChangeset()
        {
            Execute("Changeset.tmp", "Git1Changeset.txt", "Git1");
            Assert.AreEqual("ca2e4be0861b6247930a1f20b2d40ae84ac12904", ReadFirstLine("Git1Changeset.txt"));

            Execute("Changeset.tmp", "Git2Changeset.txt", "Git2");
            Assert.AreEqual("2b7c08e1784ca823ae09fef7fcafd5bdc4bff3f2", ReadFirstLine("Git2Changeset.txt"));

            Execute("Changeset.tmp", "Git3Changeset.txt", "Git3");
            Assert.AreEqual("fbfc1dc257275d928b779d9c3a073ba00b0d9701", ReadFirstLine("Git3Changeset.txt"));
        }

        [Test]
        public void Execute_WriteDirty_ShouldWriteDirty()
        {
            Execute("Dirty.tmp", "Git1Dirty.txt", "Git1");
            Assert.AreEqual("0", ReadFirstLine("Git1Dirty.txt"));

            Execute("Dirty.tmp", "Git2Dirty.txt", "Git2");
            Assert.AreEqual("1", ReadFirstLine("Git2Dirty.txt"));

            Execute("Dirty.tmp", "Git3Dirty.txt", "Git3");
            Assert.AreEqual("0", ReadFirstLine("Git3Dirty.txt"));
        }

        private static readonly string TemplatesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\Templates");
        private static readonly string RepositoriesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\Repositories");
        private static readonly string TempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\Temp");

        private static void Execute(string templateFile, string destinationFile, string workingDirectory)
        {
            GitVersionFile gitVersionFile = new GitVersionFile
            {
                TemplateFile = Path.Combine(TemplatesDirectory, templateFile),
                DestinationFile = Path.Combine(TempDirectory, destinationFile),
                WorkingDirectory = Path.Combine(TempDirectory, workingDirectory)
            };

            gitVersionFile.Execute();
        }

        private static string ReadFirstLine(string fileName)
        {
            using (StreamReader streamReader = new StreamReader(Path.Combine(TempDirectory, fileName)))
            {
                return streamReader.ReadLine();
            }
        }
    }
}
