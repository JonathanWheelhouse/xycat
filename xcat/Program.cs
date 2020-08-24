using System;
using System.IO;
using System.Linq;
using System.Text;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace xcat
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<DirectoryInfo>("--dir", "the directory to weakly encrypt")
                {
                    IsRequired = true,
                    Argument = new Argument<DirectoryInfo>().ExistingOnly()
                },

                new Option<FileInfo>("--file", "the file to hold the directory's contents")
                {
                    IsRequired = true,
                    Argument = new Argument<FileInfo>().LegalFilePathsOnly()
                }
            };

            rootCommand.Description = "Weakly encrypt a directory's contents and store in a file.";

            rootCommand.Handler = CommandHandler.Create<DirectoryInfo, FileInfo>(Main2);

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
                   sb.AppendLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> [{f.FullName}]");
                   sb.AppendLine(File.ReadAllText(f.FullName));
                   sb.AppendLine($"<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< [{f.FullName}]");
               });

            var txt = Rot13(sb.ToString());

            //txt = Rot13(txt) ;

            using (StreamWriter sw = file.CreateText())
            {
                sw.Write(txt);
            }
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

        protected static string Rot13(string value)
        {
            char[] array = value.ToCharArray();

            for (int i = 0; i < array.Length; i++)
            {
                int number = (int)array[i];

                if (number >= 'a' && number <= 'z')
                {
                    if (number > 'm')
                        number -= 13;
                    else
                        number += 13;
                }
                else
                if (number >= 'A' && number <= 'Z')
                {
                    if (number > 'M')
                        number -= 13;
                    else
                        number += 13;
                }
                array[i] = (char)number;
            }
            return new string(array);
        }


    } // end class ;
}
