using System.IO;
using NUnit.Framework;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;

namespace xycat.test;

[TestFixture]
public class YcatTest
{
    [Test]
    public void UnencryptFileCorrectly()
    {
        // arrange
        var dir = new DirectoryInfo($"{Helper.GetTestDataDirectory}{Path.DirectorySeparatorChar}expected");
        var file = new FileInfo($"{dir}{Path.DirectorySeparatorChar}rot13-expected.txt");

        var outDir = new DirectoryInfo($"{Helper.GetTestDataDirectory}{Path.DirectorySeparatorChar}out");
        Helper.EmptyDirectory(outDir);

        // act
        ycat.Program.Main2(file, outDir);

        // assert
        outDir.Refresh();

        // adir
        var aDir = new DirectoryInfo($"{Helper.GetTestDataDirectory}{Path.DirectorySeparatorChar}adir");

        var aFiles = outDir.GetFiles();
        aFiles.Count().Should().Be(2);
        aFiles.Should().Contain(f => f.Name == "a.config");
        aFiles.Should().Contain(f => f.Name == "a.sln");

        var aSubDirs = outDir.GetDirectories();
        aSubDirs.Count().Should().Be(1);

        // bdir
        var bDir = aSubDirs[0];
        bDir.Name.Should().Be("bdir");
        var bFiles = bDir.GetFiles();
        bFiles.Count().Should().Be(1);
        bFiles.Should().Contain(f => f.Name == "b.txt");

        var bSubDirs = bDir.GetDirectories();
        bSubDirs.Count().Should().Be(1);

        // cdir
        var cDir = bSubDirs[0];
        cDir.Name.Should().Be("cdir");
        var cFiles = cDir.GetFiles();
        cFiles.Count().Should().Be(1);
        cFiles.Should().Contain(f => f.Name == "c.sql");
    }

    [Test]
    public void CheckNumberOfCommandLineArgs()
    {
        var exe = GetExePath();

        var (rc, stdOut, stdErr) = Helper.Exec(exe, new List<string>());

        rc.Should().Be(1);

        stdErr.Should().Contain("Option '--file' is required");
        stdErr.Should().Contain("Option '--destDir' is required");

        stdOut.Should().Contain("Description:");
        stdOut.Should().Contain("Decrypt a file into a directory.");
        stdOut.Should().Contain("Usage:");
        stdOut.Should().Contain("ycat [options]");
        stdOut.Should().Contain("Options:");
        stdOut.Should().Contain("--file <file> (REQUIRED)        the file to decrypt");
        stdOut.Should().Contain("--destDir <destDir> (REQUIRED)  the destination directory");
        stdOut.Should().Contain("--version                       Show version information");
        stdOut.Should().Contain("-?, -h, --help                  Show help and usage information");
    }

    private static string GetExePath()
    {
        var exe = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..", "..");
        exe = Path.Combine(exe, "ycat", "bin", "net6.0");
        exe = Path.Combine(exe, "ycat");
        return exe;
    }
}
