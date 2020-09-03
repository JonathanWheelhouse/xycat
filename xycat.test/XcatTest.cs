using System.IO;
using NUnit.Framework;
using FluentAssertions;
using System.Collections.Generic;

namespace xycat.test
{
    [TestFixture]
    public class XcatTest
    {
        [Test]
        public void EncryptsDirCorrectly()
        {
            // arrange
            // ... directory to encrypt
            var dir = new DirectoryInfo($"{Helper.GetTestDataDirectory}{Path.DirectorySeparatorChar}adir");

            var outdir = new DirectoryInfo($"{Helper.GetTestDataDirectory}{Path.DirectorySeparatorChar}out");
            Helper.EmptyDirectory(outdir);

            // ... file to store the encyrpted contents of the directory
            var file = new FileInfo($"{outdir}{Path.DirectorySeparatorChar}rot13.txt");

            // act
            xcat.Program.Main2(dir, file);

            // assert
            file.Exists.Should().BeTrue();

            var expectedDir = new DirectoryInfo($"{Helper.GetTestDataDirectory}{Path.DirectorySeparatorChar}expected");
            var expectedFile = new FileInfo($"{expectedDir}{Path.DirectorySeparatorChar}rot13-expected.txt");

            Helper.FileAssert(file, expectedFile);
        }

        [Test]
        public void CheckNumberOfCommandLineArgs()
        {
            var exe = GetXcatPath();

            var (rc, stdOut, stdErr) = Helper.Exec(exe, new List<string>());

            rc.Should().Be(1);

            stdErr.Should().Contain("Option '--dir' is required");
            stdErr.Should().Contain("Option '--file' is required");

            stdOut.Should().Contain("xcat:");
            stdOut.Should().Contain("Weakly encrypt a directory's contents and store in a file.");
            stdOut.Should().Contain("Usage:");
            stdOut.Should().Contain("xcat [options]");
            stdOut.Should().Contain("Options:");
            stdOut.Should().Contain("--dir <dir> (REQUIRED)      the directory to weakly encrypt");
            stdOut.Should().Contain("--file <file> (REQUIRED)    the file to hold the directory's contents");
            stdOut.Should().Contain("--version                   Show version information");
            stdOut.Should().Contain("-?, -h, --help              Show help and usage information");
        }

        private static string GetXcatPath()
        {
            var exe = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..");
            exe = Path.Combine(exe, "xcat", "bin", "netcoreapp3.1");
            exe = Path.Combine(exe, "xcat");
            return exe;
        }
    }
}
