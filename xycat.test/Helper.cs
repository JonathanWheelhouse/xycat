using System.IO;
using NUnit.Framework;
using FluentAssertions;

public class Helper
{
    public static string GetTestDataDirectory => Path.Combine(TestContext.CurrentContext.TestDirectory, $@"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}test.data");

    public static void EmptyDirectory(DirectoryInfo directory)
    {
        foreach (var f in directory.EnumerateFiles()) { f.Delete(); }
        foreach (var d in directory.GetDirectories()) { d.Delete(true); }
    }

    public static void FileAssert(FileInfo actualFile, FileInfo expectedFile)
    {
        var actualFileLines = File.ReadAllLines(actualFile.FullName);
        var expectedFileLines = File.ReadAllLines(expectedFile.FullName);

        actualFileLines.Length.Should().Be(expectedFileLines.Length);

        // FIXME: these 2 lines should go in a library constants file
        var startLine = new string('>', 82);
        var endLine = new string('<', 82);

        for (var i = 0; i < expectedFileLines.Length; i++)
        {
            var expLine = expectedFileLines[i];
            var actLine = actualFileLines[i];

            // assert start and end lines delimiting files; file names
            if (expLine.StartsWith(startLine))
            {
                actLine.Should().StartWith(startLine);

                AssertFileName(actLine, expLine);

                continue;
            }
            if (expLine.StartsWith(endLine))
            {
                actLine.Should().StartWith(endLine);

                AssertFileName(actLine, expLine);

                continue;
            }

            actLine.Should().Be(expLine);
        }
    }

    private static void AssertFileName(string actLine, string expLine)
    {
        var expFileName = GetfileName(Between(expLine, "[", "]"));
        var actFileName = GetfileName(Between(actLine, "[", "]"));
        actFileName.Should().Be(expFileName);
    }

    private static string Between(string value, string a, string b)
    {
        int posA = value.IndexOf(a);
        int posB = value.LastIndexOf(b);
        if (posA == -1)
            return "";
        if (posB == -1)
            return "";
        int adjustedPosA = posA + a.Length;
        if (adjustedPosA >= posB)
            return "";
        return value.Substring(adjustedPosA, posB - adjustedPosA);
    }

    private static string GetfileName(string path)
    {
        var indexDirSep = path.LastIndexOfAny(new char[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
        return path.Substring(indexDirSep + 1);
    }
}
