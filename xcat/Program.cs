using System;
using System.IO;
using System.Linq;
using System.Text;
using System.CommandLine;
using xycat.common;

namespace xcat;

public class Program
{
    public static int Main(string[] args)
    {

        var dir =  new Option<DirectoryInfo>("--dir", "the directory to weakly encrypt")
        {
            IsRequired = true
        };

        var  file = new Option<FileInfo>("--file", "the file to hold the directory's contents")
        {
            IsRequired = true
        };

        var rootCommand = new RootCommand
        {
            dir,
            file
        };

        rootCommand.Description = "Weakly encrypt a directory's contents and store in a file.";

        rootCommand.SetHandler((DirectoryInfo dir, FileInfo file) => Main2(dir, file), dir, file);

        return rootCommand.InvokeAsync(args).Result;
    }

    public static void Main2(DirectoryInfo dir, FileInfo file)
    {
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
    }

    protected static bool IsGood(string filename)
    {
        var found = new string[]
        {
            ".props",
            ".csproj",
            ".config",
            ".cs",
            ".sql",
            ".pl",
            ".proj",
            ".sln",
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
            ".md",
        }
        .Any(q => filename.ToLower().EndsWith(q));

        return found;
    }
} // end class ;
