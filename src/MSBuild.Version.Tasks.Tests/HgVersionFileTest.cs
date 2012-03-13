using System;
using System.IO;
using Ionic.Zip;
using NUnit.Framework;

namespace MSBuild.Version.Tasks.Tests
{
    [TestFixture]
    public class HgVersionFileTest
    {
        [SetUp]
        public void SetUp()
        {
            // extract the repositories in the 'Hg.zip' file into the 'temp' folder
            using (ZipFile zipFile = ZipFile.Read(Path.Combine(RepositoriesDirectory, "Hg.zip")))
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
            // delete the 'temp' folder
            Directory.Delete(TempDirectory, true);
        }

        [Test]
        public void Execute_WriteChangeset_ShouldWriteChangeset()
        {
            Execute("Changeset.tmp", "Hg1Changeset.txt", "hg1");
            Assert.AreEqual("1024d08c6b37", ReadFirstLine("Hg1Changeset.txt"));

            Execute("Changeset.tmp", "Hg2Changeset.txt", "hg2");
            Assert.AreEqual("bf82f571c792", ReadFirstLine("Hg2Changeset.txt"));

            Execute("Changeset.tmp", "Hg3Changeset.txt", "hg3");
            Assert.AreEqual("80de7a096ed2", ReadFirstLine("Hg3Changeset.txt"));
        }

        [Test]
        public void Execute_WriteDirty_ShouldWriteDirty()
        {
            Execute("Dirty.tmp", "Hg1Dirty.txt", "hg1");
            Assert.AreEqual("0", ReadFirstLine("Hg1Dirty.txt"));

            Execute("Dirty.tmp", "Hg2Dirty.txt", "hg2");
            Assert.AreEqual("1", ReadFirstLine("Hg2Dirty.txt"));

            Execute("Dirty.tmp", "Hg3Dirty.txt", "hg3");
            Assert.AreEqual("1", ReadFirstLine("Hg3Dirty.txt"));
        }

        private static readonly string TemplatesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\Templates");
        private static readonly string RepositoriesDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\Repositories");
        private static readonly string TempDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data\\Temp");

        private static void Execute(string templateFile, string destinationFile, string workingDirectory)
        {
            HgVersionFile hgVersionFile = new HgVersionFile
            {
                TemplateFile = Path.Combine(TemplatesDirectory, templateFile),
                DestinationFile = Path.Combine(TempDirectory, destinationFile),
                WorkingDirectory = Path.Combine(TempDirectory, workingDirectory)
            };

            hgVersionFile.Execute();
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
