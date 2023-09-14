using CoptLib.IO;
using CoptLib.Models;
using CoptLib.ViewModels;
using CoptLib.Writing;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;

namespace CoptTest;

public class DocOps
{
    [Theory]
    [InlineData("The Morning Doxology.xml", 74, 2)]
    [InlineData("Concluding Hymn.xml", 12, 2)]
    public async Task DocLayout_Generate(string file, int rowCountEx, int colCountEx)
    {
        string xml = Resource.ReadAllText(file);
        Doc doc = DocReader.ParseDocXml(XDocument.Parse(xml));
        DocViewModel dvm = new(doc);

        var table = await dvm.CreateTableAsync();
        Assert.Equal(rowCountEx, table.Count);

        static void docTitleCheck(List<IDefinition> row) => Assert.Single(row);
        void rowCheck(List<IDefinition> row) => Assert.Equal(colCountEx, row.Count);

        var tableCheckActions = Enumerable.Repeat(rowCheck, rowCountEx - 1).Prepend(docTitleCheck);
        Assert.Collection(table, tableCheckActions.ToArray());
    }

    [Theory]
    [InlineData("The Morning Doxology.xml", 74, 2)]
    public async Task DocLayout_GenerateWithTransliteration(string file, int rowCountEx, int colCountEx)
    {
        string xml = Resource.ReadAllText(file);
        Doc doc = DocReader.ParseDocXml(XDocument.Parse(xml));

        List<LanguageInfo> transliterationTargets = new() { LanguageInfo.Parse("cop/en") };
        DocLayoutOptions layoutOptions = new()
        {
            Transliterations = transliterationTargets
        };
        DocViewModel dvm = new(new DocLayout(doc, layoutOptions));

        var table = await dvm.CreateTableAsync();
        Assert.Equal(rowCountEx, table.Count);

        static void docTitleCheck(List<IDefinition> row) => Assert.Single(row);
        void rowCheck(List<IDefinition> row) => Assert.Equal(colCountEx + transliterationTargets.Count, row.Count);

        var tableCheckActions = Enumerable.Repeat(rowCheck, rowCountEx - 1).Prepend(docTitleCheck);
        Assert.Collection(table, tableCheckActions.ToArray());
    }
}
