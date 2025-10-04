using System;
using System.IO;
using System.Linq;
using System.Text;
using System.CommandLine;
using xycat.common;

namespace xcat;

public static class Program
{
    public static int Main(string[] args)
    {

        var dirOption =  new Option<DirectoryInfo>("--dir")
        {
            Description = "the directory to weakly encrypt",
            Required = true
        };

        var  fileOption = new Option<FileInfo>("--file")
        {
            Description = "the file to hold the directory's contents",
            Required = true
        };

        var rootCommand = new RootCommand("Weakly encrypt a directory's contents and store in a file.")
        {
            dirOption,
            fileOption
        };

        rootCommand.SetAction(parseResult =>
        {
            DirectoryInfo? dir = parseResult.GetValue(dirOption);
            FileInfo? file = parseResult.GetValue(fileOption);
            return Main2(dir, file);
        });

        var parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
    }

    public static int Main2(DirectoryInfo? dir, FileInfo? file)
    {
        ArgumentNullException.ThrowIfNull(dir);
        ArgumentNullException.ThrowIfNull(file);

        Console.WriteLine(dir.FullName);

        Console.WriteLine(file.FullName);
        file.Delete();

        var sb = new StringBuilder();

        dir.GetFiles("*", SearchOption.AllDirectories)
            .Where(f => IsGood(f.FullName))
            .OrderBy(f => f.FullName)
            .ToList()
            .ForEach(f =>
            {
                Console.WriteLine(f.FullName);

                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine($"{Constant.StartLine} {Constant.StartChar}{f.FullName}{Constant.EndChar}");
                sb.AppendLine(File.ReadAllText(f.FullName));
                sb.AppendLine($"{Constant.EndLine} {Constant.StartChar}{f.FullName}{Constant.EndChar}");
            });

        var txt = Cipher.Rot13(sb.ToString());

        using StreamWriter sw = file.CreateText();
        sw.Write(txt);

        return 0;
    }

    private static readonly string[] SourceArray =
    [
        ".props",
        ".csproj",
        ".config",
        ".cs",
        ".sql",
        ".pl",
        ".proj",
        ".sln",
        "*.slnx",
        ".nuspec",
        ".json",
        ".ps1",
        ".sh",
        ".bat",
        ".cmd",
        ".txt",
        ".gitattributes",
        ".editorconfig",
        ".conf",
        ".reg",
        ".md"
    ];

    private static bool IsGood(string filename)
    {
        var found = SourceArray.Any(q => filename.ToLower().EndsWith(q));

        return found;
    }
} // end class ;
