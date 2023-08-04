using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Writing;
using CoptLib.Writing.Lexicon;
using OwlCore.Storage.SharpCompress;
using OwlCore.Storage.SystemIO;
using SharpCompress.Archives.Zip;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace CoptTest;

public class Writer
{
    private const string TestSetId = "test_set";
    private readonly string[] _testSetFileNames =
    {
        "Let Us Praise the Lord.xml",
        "The First Hoos Lobsh.xml",
        "Watos Hymn for the Three Saintly Children.xml"
    };
        
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
    public async Task DocSetWriter_CreateZipFromDocList()
    {
        var resourceName = TestSetId + ".zip";

        var setFileNew = Resource.GetTestResult(resourceName);
        using (var archive = ZipArchive.Create())
        using (ArchiveFolder setFolderNew = new(archive, resourceName, setFileNew.Name))
        {
            List<Doc> docs = new(_testSetFileNames
                .Select(Resource.ReadAllText)
                .Select(x => DocReader.ParseDocXml(x)));

            DocSetWriter setWriter = new("urn:coptlib:test-set", "Test Set", docs);
            setWriter.Set.Author = new()
            {
                FullName = "Yoshi Askharoun",
                Email = "jjask7@gmail.com",
                Website = "https://github.com/yoshiask"
            };

            await setWriter.WriteAsync(setFolderNew);

            await using var setFileStream = await setFileNew.OpenStreamAsync(FileAccess.Write);
            archive.SaveTo(setFileStream);
        }

        DocSet setActual;
        using (ArchiveFolder setFolderActual = new(Resource.GetTestResult(resourceName, FileMode.Open)))
        {
            DocSetReader readerActual = new(setFolderActual);
            await readerActual.ReadDocs();
            setActual = readerActual.Set;
        }

        DocSet setExpected;
        using (ArchiveFolder setFolderExpected = new(Resource.Get(resourceName)))
        {
            DocSetReader readerExpected = new(setFolderExpected);
            await readerExpected.ReadDocs();
            setExpected = readerExpected.Set;
        }

        Assert.Equal(setExpected.Name, setActual.Name);
        Assert.Equal(setExpected.Key, setActual.Key);
        Assert.Equal(setExpected.Author?.Email, setActual.Author?.Email);
        Assert.Equal(setExpected.Author?.Website, setActual.Author?.Website);
        Assert.Equal(setExpected.IncludedDocs.Count, setActual.IncludedDocs.Count);
    }

    [Fact]
    public async Task DocSetWriter_CreateFolderFromDocList()
    {
        var setFolderActualPath = Resource.TestResultPath(TestSetId);
        Directory.CreateDirectory(setFolderActualPath);
        SystemFolder setFolderNew = new(setFolderActualPath);
        {
            List<Doc> docs = new(_testSetFileNames
                .Select(Resource.ReadAllText)
                .Select(x => DocReader.ParseDocXml(x)));

            DocSetWriter setWriter = new("urn:coptlib:test-set", "Test Set", docs);
            setWriter.Set.Author = new()
            {
                FullName = "Yoshi Askharoun",
                Email = "jjask7@gmail.com",
                Website = "https://github.com/yoshiask"
            };

            await setWriter.WriteAsync(setFolderNew);
        }

        DocSet setActual;
        SystemFolder setFolderActual = new(setFolderActualPath);
        {
            DocSetReader readerActual = new(setFolderActual);
            await readerActual.ReadDocs();
            setActual = readerActual.Set;
        }

        DocSet setExpected;
        SystemFolder setFolderExpected = new(Resource.Path(TestSetId));
        {
            DocSetReader readerExpected = new(setFolderExpected);
            await readerExpected.ReadDocs();
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

    [Theory]
    [InlineData("Hina `ntenhwc `erok@ `nnoytoc nem Dauid@ enws oubyk@ ouoh enjw `mmoc.", "CopticStandard",
        "Ϩⲓⲛⲁ ⲛ\u0300ⲧⲉⲛϩⲱⲥ ⲉ\u0300ⲣⲟⲕ: ⲛ\u0300ⲛⲟⲏⲧⲟⲥ ⲛⲉⲙ Ⲇⲁⲩⲓⲇ: ⲉⲛⲱϣ ⲟⲩⲃⲏⲕ: ⲟⲩⲟϩ ⲉⲛϫⲱ ⲙ\u0300ⲙⲟⲥ.", "Unicode")]
    [InlineData("Ⲡⲓⲡ\u0300ⲛⲉⲩⲙⲁ", "Unicode", "Ⲡⲓ\u0300ⲡⲛⲉⲩⲙⲁ", "UnicodeB")]
    public void DisplayFont_Convert(string inputText, string sourceMapId, string outputTextEx, string targetMapId)
    {
        if (!DisplayFont.TryFindFontByMapId(sourceMapId, out var sourceFont))
            Assert.Fail($"No font supporting the '{sourceMapId}' mapping was found.");
            
        if (!DisplayFont.TryFindFontByMapId(targetMapId, out var targetFont))
            Assert.Fail($"No font supporting the '{targetMapId}' mapping was found.");

        var outputTextAc = sourceFont.Convert(inputText, targetFont);
        Assert.NotNull(outputTextAc);
        Assert.Equal(outputTextEx, outputTextAc);
    }

    [Theory]
    [InlineData("ⲍⲉⲛⲍⲉⲛ", new[] { "ⲍⲉⲛⲍⲉⲛ" }, "lizard, chameleon", 0)]
    [InlineData("ⲁⲛⲑⲟⲩⲥ", new[] { "ⲁⲛⲑⲟⲩⲥ" }, "(species of) lizard", 0)]
    [InlineData("ⲁⲣⲭⲓⲉⲣⲉⲩⲥ", new[] { "ⲁⲣⲭⲓⲉⲣⲉⲩⲥ", "ⲁⲣϫⲓⲉⲣⲉⲩⲥ", "ⲁⲣⲭⲓⲉⲣⲉⲩⲉ", "ⲁⲣⲭⲓⲉⲣⲉⲱⲥ", "ⲁⲣϫⲓⲉⲣⲉⲱⲥ", "ⲁⲣⲭⲏⲉⲣⲉⲩⲥ" }, "high priest (of Melchizedek)", 2)]
    public async Task Lexicon_TeiKellia(string query, string[] orthography, string enDefinitionEx, int senseIndex)
    {
        var lexicon = await GetKelliaLexicon();
            
        var result = await lexicon.SearchAsync(query, new LanguageInfo(KnownLanguage.Coptic));
        Assert.NotNull(result);

        HashSet<string> orthSetAc = new(result.Forms.Select(f => f.Orthography));
        orthSetAc.SymmetricExceptWith(orthography);
        Assert.Empty(orthSetAc);

        var enDefinitionAc = result.Senses[senseIndex].Translations
            .GetByLanguage(KnownLanguage.English)?.ToString();
        Assert.NotNull(enDefinitionAc);
        Assert.Equal(enDefinitionEx, enDefinitionAc);
    }

    private ILexicon? _kelliaLexicon;
    private async Task<ILexicon> GetKelliaLexicon()
    {
        if (_kelliaLexicon is null)
        {
            XDocument teiDoc;
            var assembly = typeof(ILexicon).GetTypeInfo().Assembly;
            await using (var teiStream =
                         assembly.GetManifestResourceStream(
                             $"{assembly.GetName().Name}.Resources.Comprehensive_Coptic_Lexicon-v1.2-2020.xml"))
            {
                Assert.NotNull(teiStream);
                teiDoc = await XDocument.LoadAsync(teiStream, LoadOptions.None, default);
            }

            _kelliaLexicon = new TeiLexicon(new(KnownLanguage.Coptic), teiDoc, null);
        }

        return _kelliaLexicon;
    }
}