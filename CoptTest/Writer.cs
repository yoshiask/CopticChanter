using CoptLib.IO;
using CoptLib.Models;
using OwlCore.Storage.SystemIO.Compression;
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
            Assert.Equal(docEx.Uuid, docAc.Uuid);
            Assert.Equal(docEx.Author?.FullName, docAc.Author?.FullName);
            Assert.Equal(docEx.DirectDefinitions.Count, docAc.DirectDefinitions.Count);
            Assert.Equal(docEx.Translations.Children.Count, docAc.Translations.Children.Count);
            Assert.Equal(docEx.Context.Definitions.Count, docAc.Context.Definitions.Count);
        }

        [Fact]
        public async Task DocSetWriter_CreateFromDocList()
        {
            string resourceName = "test_set.zip";
            string[] fileNames = new[]
            {
                "Let Us Praise the Lord.xml",
                "The First Hoos Lobsh.xml",
                "Watos Hymn for the Three Saintly Children.xml"
            };

            using (var setStreamActual = Resource.OpenTestResult(resourceName))
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
                setWriter.Write(setStreamActual);
            }

            DocSet setActual;
            using (var setStreamActual = Resource.OpenTestResult(resourceName, FileMode.Open))
            using (ZipArchive setArchiveActual = new(setStreamActual))
            {
                ZipArchiveFolder setFolderActual = new(Path.GetFileNameWithoutExtension(resourceName), resourceName, setArchiveActual);
                DocSetReader readerActual = new(setFolderActual);
                await readerActual.ReadAll();
                setActual = readerActual.Set;
            }

            DocSet setExpected;
            using (var setStreamExpected = Resource.Open(resourceName))
            using (ZipArchive setArchiveExpected = new(setStreamExpected))
            {
                ZipArchiveFolder setFolderExpected = new(Path.GetFileNameWithoutExtension(resourceName), resourceName, setArchiveExpected);
                DocSetReader readerExpected = new(setFolderExpected);
                await readerExpected.ReadAll();
                setExpected = readerExpected.Set;
            }

            Assert.Equal(setExpected.Name, setActual.Name);
            Assert.Equal(setExpected.Uuid, setActual.Uuid);
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
            DocReader.ApplyDocTransforms(doc);

            using (var texStreamActual = Resource.OpenTestResult($"{resourceName}.tex"))
            {
                Tex.WriteTex(doc, texStreamActual);
            }
        }
    }
}
