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
            Assert.AreEqual("0", ReadFirstLine("Hg1DirtyBuild.txt"));

            Execute("DirtyBuild.tmp", "Hg2DirtyBuild.txt", "hg2");
            Assert.AreEqual("1", ReadFirstLine("Hg2DirtyBuild.txt"));

            Execute("DirtyBuild.tmp", "Hg3DirtyBuild.txt", "hg3");
            Assert.AreEqual("1", ReadFirstLine("Hg3DirtyBuild.txt"));
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
