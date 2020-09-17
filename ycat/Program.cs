using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.CommandLine;
using System.CommandLine.Invocation;
using xycat.common;

namespace ycat
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<FileInfo>("--file", "the file to decrypt")
                {
                    IsRequired = true,
                    Argument = new Argument<FileInfo>().ExistingOnly()
                },
                new Option<DirectoryInfo>("--destDir", "the destination directory")
                {
                    IsRequired = true,
                    Argument = new Argument<DirectoryInfo>().ExistingOnly()
                }
            };

            rootCommand.Description = "Decrypt a file into a directory.";

            rootCommand.Handler = CommandHandler.Create<FileInfo, DirectoryInfo>(Main2);

            return rootCommand.InvokeAsync(args).Result;
        }

        public static void Main2(FileInfo file, DirectoryInfo destDir)
        {
            var txt = File.ReadAllText(file.FullName);

            var unrot13 = Cipher.Rot13(txt);

            const string startLine = ">>>>>>>>>>";
            const string endLine = "<<<<<<<<<<";
            var sourceTopLevelDirectory = GetSourceTopLevelDirectory(unrot13, startLine);

            var lines = unrot13.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var lines2 = lines.SkipWhile(l => string.IsNullOrEmpty(l));

            // split into groups of lines (divided by start of a new file); each group is 1 file
            var fileLines = lines2
                .Aggregate(new[] { new List<string>() }.ToList(), (a, line) =>
                {
                    if (line.StartsWith(startLine) && a.Last().Any())
                        a.Add(new List<string>());
                    a.Last().Add(line);
                    return a;
                })
                .ToList();

            fileLines.ForEach(f =>
            {
                // get source path
                var fileLines2 = f.SkipWhile(l => !l.StartsWith(startLine));
                var pathLine = fileLines2.First();
                var path = Between(pathLine, "[", "]");

                // transform path to operating system file path with dest folder
                var destPath = path.Replace(sourceTopLevelDirectory, "");
                destPath = destPath.TrimStart('\\', '/');
                destPath = destPath.Replace('\\', Path.DirectorySeparatorChar);
                destPath = destPath.Replace('/', Path.DirectorySeparatorChar);
                destPath = Path.Combine(destDir.FullName, destPath);

                // write the file
                var fileLines3 = fileLines2
                    .Skip(1)  // skip pathLine
                    .TakeWhile(l => !l.StartsWith(endLine)); // skip endLine
                var dir = Path.GetDirectoryName(destPath);
                Directory.CreateDirectory(dir);
                File.WriteAllLines(destPath, fileLines3);
            });
        }

        private static string GetSourceTopLevelDirectory(string unrot13, string startLine)
        {
            var list = new List<string>(Regex.Split(unrot13, Environment.NewLine));
            var pathlines = list.Where(l => l.StartsWith(startLine)).ToList();
            var paths = pathlines.Select(l => Between(l, "[", "]")).ToList();
            var smallestPath = paths.OrderBy(d => d.Length).First();

            // FIXME: not always correct
            var indexDirSep = smallestPath.LastIndexOfAny(new char[] { '\\', '/' });

            var smallestDir = smallestPath.Substring(0, indexDirSep);
            Console.WriteLine($"Source top level directory: {smallestDir}");
            return smallestDir;
        }

        /// <summary>
        /// Get string value between [first] a and [last] b.
        /// </summary>
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

    } // end class ;
}
