using System.IO;
using System.Collections.ObjectModel;
using NUnit.Framework;
using FluentAssertions;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System;
using xycat.common;

public class Helper
{
    public static string GetTestDataDirectory => Path.Combine(TestContext.CurrentContext.TestDirectory, $@"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}test.data");

    public static void EmptyDirectory(DirectoryInfo directory)
    {
        if (directory.Exists)
            directory.Delete(true);
        directory.Create();
    }

    public static (int returnCode, string standardOut, string standardError) Exec(string fileName, List<string> arguments)
    {
        Console.WriteLine($"Exec '{fileName}' {string.Join(" ", arguments)}");

        var psi = new ProcessStartInfo();
        psi.FileName = fileName;
        psi.Arguments = "";
        arguments.ForEach(a => psi.ArgumentList.Add(a));
        psi.WorkingDirectory = Path.GetDirectoryName(fileName);
        psi.CreateNoWindow = true;
        psi.UseShellExecute = false;

        var sbStdOut = new StringBuilder();
        var sbStdErr = new StringBuilder();

        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;

        using var proc = new Process();
        proc.StartInfo = psi;
        proc.EnableRaisingEvents = true;
        proc.OutputDataReceived += new DataReceivedEventHandler((sender, e) => { sbStdOut.AppendLine(e.Data); });
        proc.ErrorDataReceived += new DataReceivedEventHandler((sender, e) => { sbStdErr.AppendLine(e.Data); });

        proc.Start();

        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();

        proc.WaitForExit();

        Console.WriteLine($"rc {proc.ExitCode}");

        return (proc.ExitCode, sbStdOut.ToString().Trim(), sbStdErr.ToString().Trim());
    }

    public static void FileAssert(FileInfo actualFile, FileInfo expectedFile)
    {
        var actualFileLines = File.ReadAllLines(actualFile.FullName);
        var expectedFileLines = File.ReadAllLines(expectedFile.FullName);

        actualFileLines.Length.Should().Be(expectedFileLines.Length);

        for (var i = 0; i < expectedFileLines.Length; i++)
        {
            var expLine = expectedFileLines[i];
            var actLine = actualFileLines[i];

            // assert start and end lines delimiting files; file names
            if (expLine.StartsWith(Constant.StartLine))
            {
                actLine.Should().StartWith(Constant.StartLine);

                AssertFileName(actLine, expLine);

                continue;
            }
            if (expLine.StartsWith(Constant.EndLine))
            {
                actLine.Should().StartWith(Constant.EndLine);

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
        var indexDirSep = path.LastIndexOfAny(new char[] { '\\', '/' });
        return path.Substring(indexDirSep + 1);
    }
}
