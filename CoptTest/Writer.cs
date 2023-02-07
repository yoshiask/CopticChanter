using CoptLib.IO;
using CoptLib.Models;
using OwlCore.Storage.Archive;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace CoptTest
{
    public class Writer
    {
        [Theory]
        [InlineData("Let Us Praise the Lord.xml")]
        [InlineData("The First Hoos Lobsh.xml")]
        [InlineData("Watos Hymn for the Three Saintly Children.xml")]
        public void DocWriter_OpenAndWriteNoChanges(string file)
        {
            string xmlEx = Resource.ReadAllText(file);
            Doc docEx = DocReader.ParseDocXml(XDocument.Parse(xmlEx));

            string xmlAc = DocWriter.WriteDocXml(docEx);
            Doc docAc = DocReader.ParseDocXml(XDocument.Parse(xmlAc));

            Assert.Equal(docEx.Name, docAc.Name);
            Assert.Equal(docEx.Key, docAc.Key);
            Assert.Equal(docEx.Author?.FullName, docAc.Author?.FullName);
            Assert.Equal(docEx.DirectDefinitions.Count, docAc.DirectDefinitions.Count);
            Assert.Equal(docEx.Translations.Children.Count, docAc.Translations.Children.Count);
            Assert.Equal(docEx.Context.Definitions.Count, docAc.Context.Definitions.Count);
        }

        [Fact]
        public async Task DocSetWriter_CreateFromDocList()
        {
            const string resourceName = "test_set.zip";
            string[] fileNames =
            {
                "Let Us Praise the Lord.xml",
                "The First Hoos Lobsh.xml",
                "Watos Hymn for the Three Saintly Children.xml"
            };

            using (ZipArchiveFolder setFolderActual = new(Resource.GetTestResult(resourceName)))
            {
                List<Doc> docs = new(fileNames
                    .Select(Resource.ReadAllText)
                    .Select(x => DocReader.ParseDocXml(x)));

                DocSetWriter setWriter = new("test-set", "Test Set", docs);
                setWriter.Set.Author = new()
                {
                    FullName = "Yoshi Askharoun",
                    Email = "jjask7@gmail.com",
                    Website = "https://github.com/yoshiask"
                };
                await setWriter.Write(setFolderActual);
            }

            DocSet setActual;
            using (ZipArchiveFolder setFolderActual = new(Resource.GetTestResult(resourceName, FileMode.Open)))
            {
                DocSetReader readerActual = new(setFolderActual);
                await readerActual.ReadAll();
                setActual = readerActual.Set;
            }

            DocSet setExpected;
            using (ZipArchiveFolder setFolderExpected = new(Resource.Get(resourceName)))
            {
                DocSetReader readerExpected = new(setFolderExpected);
                await readerExpected.ReadAll();
                setExpected = readerExpected.Set;
            }

            Assert.Equal(setExpected.Name, setActual.Name);
            Assert.Equal(setExpected.Key, setActual.Key);
            Assert.Equal(setExpected.Author?.Email, setActual.Author?.Email);
            Assert.Equal(setExpected.Author?.Website, setActual.Author?.Website);
            Assert.Equal(setExpected.IncludedDocs.Count, setActual.IncludedDocs.Count);
        }

        [Theory]
        [InlineData("The First Hoos Lobsh")]
        [InlineData("Hymn of the Seven Tunes")]
        public void TexWriter(string resourceName)
        {
            string xml = Resource.ReadAllText($"{resourceName}.xml");
            Doc doc = DocReader.ParseDocXml(xml);
            doc.ApplyTransforms();

            using (var texStreamActual = Resource.OpenTestResult($"{resourceName}.tex"))
            {
                Tex.WriteTex(doc, texStreamActual);
            }
        }
    }
}