using CoptLib.IO;
using CoptLib.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Xunit;

namespace CoptTest
{
    public class Writer
    {
        readonly string _resPrefix;

        public Writer()
        {
            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var dirPath = Path.GetDirectoryName(codeBasePath);
            _resPrefix = Path.Combine(dirPath, @"Resources\");
        }

        [Theory]
        [InlineData("Let Us Praise the Lord.xml")]
        [InlineData("The First Hoos Lobsh.xml")]
        [InlineData("Watos Hymn for the Three Saintly Children.xml")]
        public void DocWriter_OpenAndWriteNoChanges(string file)
        {
            string xmlEx = File.ReadAllText(_resPrefix + file);
            Doc docEx = DocReader.ParseDocXml(XDocument.Parse(xmlEx));

            string xmlAc = DocWriter.WriteDocXml(docEx);
            Doc docAc = DocReader.ParseDocXml(XDocument.Parse(xmlAc));

            Assert.Equal(docEx.Name, docAc.Name);
            Assert.Equal(docEx.Uuid, docAc.Uuid);
            Assert.Equal(docEx.Author?.FullName, docAc.Author?.FullName);
            Assert.Equal(docEx.Definitions.Count, docAc.Definitions.Count);
            Assert.Equal(docEx.DirectDefinitions.Count, docAc.DirectDefinitions.Count);
            Assert.Equal(docEx.Translations.Children.Count, docAc.Translations.Children.Count);
        }

        [Fact]
        public void DocSetWriter_CreateFromDocList()
        {
            string pathActual = _resPrefix + "..\\Output\\test_set.zip";
            string pathExpected = _resPrefix + "test_set.zip";
            string[] fileNames = new[]
            {
                "Let Us Praise the Lord.xml",
                "The First Hoos Lobsh.xml",
                "Watos Hymn for the Three Saintly Children.xml"
            };

            List<Doc> docs = new(fileNames.Select(f => DocReader.ReadDocXml(_resPrefix + f)));
            DocSetWriter setWriter = new("test-set", "Test Set", docs);
            setWriter.Set.Author = new()
            {
                FullName = "Yoshi Askharoun",
                Email = "jjask7@gmail.com",
                Website = "https://github.com/yoshiask"
            };
            setWriter.Write(pathActual);

            DocSet setActual;
            using (DocSetReader readerActual = new(pathActual))
            {
                readerActual.ReadAll();
                setActual = readerActual.Set;
            }

            DocSet setExpected;
            using (DocSetReader readerExpected = new(pathExpected))
            {
                readerExpected.ReadAll();
                setExpected = readerExpected.Set;
            }

            Assert.Equal(setExpected.Name, setActual.Name);
            Assert.Equal(setExpected.Uuid, setActual.Uuid);
            Assert.Equal(setExpected.Author?.Email, setActual.Author?.Email);
            Assert.Equal(setExpected.Author?.Website, setActual.Author?.Website);
            Assert.Equal(setExpected.IncludedDocs.Count, setActual.IncludedDocs.Count);
        }
    }
}
