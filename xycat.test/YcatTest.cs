using System.IO;
using NUnit.Framework;
using FluentAssertions;
using System.Collections.Generic;

namespace xycat.test
{
    [TestFixture]
    public class YcatTest
    {
        [Test]
        public void CheckNumberOfCommandLineArgs()
        {
            var exe = GetYcatPath();

            var (rc, stdOut, stdErr) = Helper.Exec(exe, new List<string>());

            rc.Should().Be(1);

            stdErr.Should().Contain("Option '--file' is required");
            stdErr.Should().Contain("Option '--destDir' is required");

            stdOut.Should().Contain("ycat:");
            stdOut.Should().Contain("Decrypt a file into a directory.");
            stdOut.Should().Contain("Usage:");
            stdOut.Should().Contain("ycat [options]");
            stdOut.Should().Contain("Options:");
            stdOut.Should().Contain("--file <file> (REQUIRED)          the file to decrypt");
            stdOut.Should().Contain("--destDir <destdir> (REQUIRED)    the destination directory");
            stdOut.Should().Contain("--version                         Show version information");
            stdOut.Should().Contain("-?, -h, --help                    Show help and usage information");
        }

        private static string GetYcatPath()
        {
            var exe = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..");
            exe = Path.Combine(exe, "ycat", "bin", "netcoreapp3.1");
            exe = Path.Combine(exe, "ycat");
            return exe;
        }
    }
}
