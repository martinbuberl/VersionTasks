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
        public void Execute_WriteRevision_ShouldWriteRevision()
        {
            Execute("Revision.tmp", "Hg1Revision.txt", "hg1");
            Assert.AreEqual(ReadFirstLine("Hg1Revision.txt"), "2");

            Execute("Revision.tmp", "Hg2Revision.txt", "hg2");
            Assert.AreEqual(ReadFirstLine("Hg2Revision.txt"), "4");

            Execute("Revision.tmp", "Hg3Revision.txt", "hg3");
            Assert.AreEqual(ReadFirstLine("Hg3Revision.txt"), "2");
        }

        [Test]
        public void Execute_WriteChangeset_ShouldWriteChangeset()
        {
            Execute("Changeset.tmp", "Hg1Changeset.txt", "hg1");
            Assert.AreEqual(ReadFirstLine("Hg1Changeset.txt"), "1024d08c6b37");

            Execute("Changeset.tmp", "Hg2Changeset.txt", "hg2");
            Assert.AreEqual(ReadFirstLine("Hg2Changeset.txt"), "bf82f571c792");

            Execute("Changeset.tmp", "Hg3Changeset.txt", "hg3");
            Assert.AreEqual(ReadFirstLine("Hg3Changeset.txt"), "80de7a096ed2");
        }

        [Test]
        public void Execute_WriteDirty_ShouldWriteDirty()
        {
            Execute("Dirty.tmp", "Hg1Dirty.txt", "hg1");
            Assert.AreEqual(ReadFirstLine("Hg1Dirty.txt"), "0");

            Execute("Dirty.tmp", "Hg2Dirty.txt", "hg2");
            Assert.AreEqual(ReadFirstLine("Hg2Dirty.txt"), "1");

            Execute("Dirty.tmp", "Hg3Dirty.txt", "hg3");
            Assert.AreEqual(ReadFirstLine("Hg3Dirty.txt"), "1");
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
