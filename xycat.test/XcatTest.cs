using System;
using System.IO;
using NUnit.Framework;
using FluentAssertions;

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
        public void ChecksNumberOfCommandLineArgs()
        {
            true.Should().BeTrue();
        }

        [Test]
        public void atest()
        {
            1.Should().Be(1);
        }
    }
}
