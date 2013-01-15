using System;
using System.IO;
using Ionic.Zip;
using NUnit.Framework;

namespace VersionTasks.Tests
{
    [TestFixture]
    public class HgVersionFileTest
    {
        private static string _templatesDirectory;
        private static string _repositoriesDirectory;
        private static string _tempDirectory;

        [SetUp]
        public void SetUp()
        {
            _templatesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\Templates");
            _repositoriesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\Repositories");
            _tempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\Temp", Guid.NewGuid().ToString());

            // extract the repositories in the 'Hg.zip' file into the 'temp' folder
            using (ZipFile zipFile = ZipFile.Read(Path.Combine(_repositoriesDirectory, "Hg.zip")))
            {
                foreach (ZipEntry zipEntry in zipFile)
                {
                    zipEntry.Extract(_tempDirectory, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            // in combination with TortoiseHg "Directory.Delete()' throws an "System.UnauthorizedAccessException'
            SafeDirectoryDelete(_tempDirectory);
        }

        [Test]
        public void Execute_WriteChangeset_ShouldWriteChangeset()
        {
            Execute("Changeset.tmp", "Hg1Changeset.txt", "hg1");
            Assert.AreEqual("1024d08c6b3733bd3b0a346e485d1ecd64183eeb", ReadFirstLine("Hg1Changeset.txt"));

            Execute("Changeset.tmp", "Hg2Changeset.txt", "hg2");
            Assert.AreEqual("bf82f571c7928bc7078b8b8413b601d17fa04cbd", ReadFirstLine("Hg2Changeset.txt"));

            Execute("Changeset.tmp", "Hg3Changeset.txt", "hg3");
            Assert.AreEqual("80de7a096ed2d19142a946024165542c043971bf", ReadFirstLine("Hg3Changeset.txt"));
        }

        [Test]
        public void Execute_WriteChangesetShort_ShouldWriteChangesetShort()
        {
            Execute("ChangesetShort.tmp", "Hg1ChangesetShort.txt", "hg1");
            Assert.AreEqual("1024d08c6b37", ReadFirstLine("Hg1ChangesetShort.txt"));

            Execute("ChangesetShort.tmp", "Hg2ChangesetShort.txt", "hg2");
            Assert.AreEqual("bf82f571c792", ReadFirstLine("Hg2ChangesetShort.txt"));

            Execute("ChangesetShort.tmp", "Hg3ChangesetShort.txt", "hg3");
            Assert.AreEqual("80de7a096ed2", ReadFirstLine("Hg3ChangesetShort.txt"));
        }

        [Test]
        public void Execute_WriteDirtyBuild_ShouldWriteDirtyBuild()
        {
            Execute("DirtyBuild.tmp", "Hg1DirtyBuild.txt", "hg1");
            Assert.AreEqual("false", ReadFirstLine("Hg1DirtyBuild.txt"));

            Execute("DirtyBuild.tmp", "Hg2DirtyBuild.txt", "hg2");
            Assert.AreEqual("true", ReadFirstLine("Hg2DirtyBuild.txt"));

            Execute("DirtyBuild.tmp", "Hg3DirtyBuild.txt", "hg3");
            Assert.AreEqual("true", ReadFirstLine("Hg3DirtyBuild.txt"));
        }

        private static void Execute(string templateFile, string destinationFile, string workingDirectory)
        {
            HgVersionFile hgVersionFile = new HgVersionFile
            {
                TemplateFile = Path.Combine(_templatesDirectory, templateFile),
                DestinationFile = Path.Combine(_tempDirectory, destinationFile),
                WorkingDirectory = Path.Combine(_tempDirectory, workingDirectory)
            };

            hgVersionFile.Execute();
        }

        private static string ReadFirstLine(string fileName)
        {
            using (StreamReader streamReader = new StreamReader(Path.Combine(_tempDirectory, fileName)))
            {
                return streamReader.ReadLine();
            }
        }

        /// <see href="http://stackoverflow.com/a/8521573/135441" />
        private static void SafeDirectoryDelete(string targetDir)
        {
            File.SetAttributes(targetDir, FileAttributes.Normal);

            string[] files = Directory.GetFiles(targetDir);
            string[] dirs = Directory.GetDirectories(targetDir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                SafeDirectoryDelete(dir);
            }

            Directory.Delete(targetDir, false);
        }
    }
}
