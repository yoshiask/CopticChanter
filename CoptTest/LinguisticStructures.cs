using CoptLib.Extensions;
using CoptLib.Writing.Lexicon;
using CoptLib.Writing.Linguistics;
using CoptLib.Writing.Linguistics.Analyzers;
using CoptLib.Writing.Linguistics.XBar;
using OwlCore.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace CoptTest;

public class LinguisticStructures(ITestOutputHelper _output)
{
    private readonly CopticBohairicTranslator _bohairicTranslator = new();
    private readonly ITranslator _translator = new CopticBohairicTranslator();

    [Fact]
    public void BinaryTree_GraphViz()
    {
        BinaryNode<string> root = new("XP",
            new("Specifier"),
                new("X'",
                new("X"),
                    new("Complement")
            )
        );

        _output.WriteLine(root.SerializeToDot());
    }

    [Fact]
    public void RuleSet()
    {
        Tag rootTag = new(PhrasalCategory.Clause, XBarNodeType.Phrase);
        RuleSet rules = new([rootTag], [
            new(new(PhrasalCategory.Tense, XBarNodeType.Phrase), ["{NP/CP}", "(T)", "VP"])
        ]);
    }

    [Theory]
    [InlineData("ϯⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲕⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲧⲉⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ϥⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲥⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲧⲉⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲥⲉⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲕⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲣⲉⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉϥⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲥⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲣⲉⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲩⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉϯⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲉⲕⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲉⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲉϥⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲉⲥⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲉⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲟⲩⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲕⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲣⲉⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁϥⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲥⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲣⲉⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲩⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲕⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲣⲉⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁϥⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲥⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲣⲉⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲩⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ϯⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲭⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲧⲉⲣⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ϥⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲥⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲧⲉⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲧⲉⲧⲉⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲥⲉⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲓⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲭⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲣⲉⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉϥⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲥⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲣⲉⲧⲉⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲩⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉϯⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲉⲭⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲉⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲉϥⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲉⲥⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲉⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲉⲧⲉⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲉⲧⲟⲩⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲓⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲭⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲣⲉⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁϥⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲥⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲣⲉⲧⲉⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲛⲁⲩⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲓⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲭⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲣⲉⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁϥⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲥⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲣⲉⲧⲉⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲁⲩⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]

    [InlineData("ⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Past)]
    [InlineData("ⲙⲡⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲧⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲧⲙⲡⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲉⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲉⲙⲡⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲧⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲙⲡⲁϯⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲁϯⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲧⲙⲡⲁϯⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲁⲙⲡⲁϯⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ϣⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲙⲡⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉϣⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲧϣⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲧⲙⲡⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲁϣⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲁⲙⲡⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲓⲉⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲙⲁⲣⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲙⲡⲉⲛⲑⲣⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲧⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲧⲁⲣⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ϣⲁϯⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲁⲓϣⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲁⲓⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]

    [InlineData("ⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲧⲉⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲉⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲁⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲁⲛⲛⲁⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲙⲡⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲙⲡⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲉⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲉⲙⲡⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲙⲡⲁⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲁⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲙⲡⲁⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲁⲙⲡⲁⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ϣⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲙⲡⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉϣⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧϣⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲙⲡⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲁϣⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲁⲙⲡⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲛⲉⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲛⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲙⲁⲣⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲙⲡⲉⲛⲑⲣⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲧⲁⲣⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ϣⲁⲧⲉⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲁⲛϣⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲁⲛⲥⲱⲧⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    public async Task BohairicCoptic_DetectVerbs(string verb, PointOfView pov, GrammaticalCount number, Gender gender, RelativeTime start = default, RelativeTime end = default, TenseFlags flags = default)
    {
        verb = CopticBohairicTranslator.NormalizeText(verb);
        
        var interpretationsAsync = _bohairicTranslator.IdentifyVerb(verb);
        var interpretations = await interpretationsAsync.ToListAsync();
        Assert.NotEmpty(interpretations);
        
        foreach (var x in interpretations)
        {
            var text = string.Join("⸱", x.Select(y => verb.Substring(y.SourceRange)));
            var annotations = string.Join(", ", x.Select(y => y.ToString()));

            _output.WriteLine(text);
            _output.WriteLine(annotations);
            //_output.WriteLine(DemoText(x));
            _output.WriteLine("");
        }
    }

    [Theory]
    [InlineData("ⲛⲉⲛⲓⲟϯ")]
    [InlineData("Ⲫⲓⲱⲧ")]
    [InlineData("ⲙⲪⲓⲱⲧ")]
    [InlineData("ⲛⲪⲓⲱⲧ")]
    [InlineData("Ⲡϭⲟⲓⲥ")]
    [InlineData("Ⲡⲉⲛϭⲟⲓⲥ")]
    [InlineData("ⲡⲁϭⲟⲓⲥ")]
    [InlineData("ⲧⲁϭⲟⲓⲥ")]
    [InlineData("ⲡⲓⲙⲁ")]
    [InlineData("ⲡⲓⲙⲁⲛϣⲉⲗⲉⲧ")]
    [InlineData("ⲟⲩⲣⲉϥⲉⲣⲛⲟⲃⲓ")]
    [InlineData("ϯⲙⲉⲧⲣⲉⲙⲛ̀ⲭⲏⲙⲓ")]
    public async Task BohairicCoptic_DetectNouns(string noun)
    {
        noun = CopticBohairicTranslator.NormalizeText(noun);

        var interpretations = _bohairicTranslator.IdentifyNoun(noun);
        await foreach (var x in interpretations)
        {
            var text = string.Join("⸱", x.Select(y => noun.Substring(y.SourceRange)));
            var annotations = string.Join(", ", x.Select(y => y.ToString()));

            _output.WriteLine(text);
            _output.WriteLine(annotations);
            _output.WriteLine(DemoText(x));
            _output.WriteLine("");
        }
    }

    [Theory]
    [InlineData("ⲡ̀ⲏⲓ ⲛ̀ⲧⲉ ⲛⲓⲁ̀ⲅⲅⲉⲗⲟⲥ")]
    [InlineData("Ϧⲉⲛ Ⲡⲓⲭⲣⲓⲥⲧⲟⲥ Ⲓⲏⲥⲟⲩⲥ Ⲡⲉⲛϭⲟⲓⲥ")]
    [InlineData("Ϣⲟⲡ ⲁⲧϭⲛⲉ ⲟⲩⲙⲉⲧⲕⲟⲩϫⲓ ⲛϩⲏⲧ!")]
    [InlineData("Ⲓⲏⲥⲟⲩⲥ Ⲡⲓⲭⲣⲓⲥⲧⲟⲥ Ⲡⲟⲩⲣⲟ ⲛⲧⲉ Ⲡⲱⲟⲩ: ⲁϥⲧⲱⲛϥ ⲉⲃⲟⲗ ϧⲉⲛ ⲛⲏⲉⲑⲙⲱⲟⲩⲧ.")]
    [InlineData("Ⲡⲁϭⲟⲓⲥ Ⲓⲏⲥⲟⲩⲥ Ⲡⲓⲭⲣⲓⲥⲧⲟⲥ: Ⲡϣⲏⲣⲓ ⲙⲪⲛⲟⲩϯ: ⲛⲁⲓ ⲛⲏⲓ: ⲟⲩⲣⲉϥⲉⲣⲛⲟⲃⲓ.")]
    [InlineData("Ⲭⲉⲣⲉ ⲡⲓⲙⲁⲛ̀ϣⲉⲗⲉⲧ: ⲉⲧⲥⲉⲗⲥⲱⲗ ϧⲉⲛ ⲟⲩⲑⲟ ⲛ̀ⲣⲏϯ: ⲛ̀ⲧⲉ Ⲡⲓⲛⲩⲙⲫⲓⲟⲥ ⲙ̀ⲙⲏⲓ: ⲉ̀ⲧⲁϥϩⲱⲧⲡ ⲉ̀ϯⲙⲉⲧⲣⲱⲙⲓ.")]
    [InlineData("Ⲁ̀ⲛⲟⲛ ϩⲱⲛ ⲙⲁⲣⲉⲛⲟⲩⲱϣⲧ ⲙ̀ⲙⲟϥ: ⲉⲛⲱϣ ⲉ̀ⲃⲟⲗ ⲉⲛϫⲱ ⲙ̀ⲙⲟⲥ: ϫⲉ ⲛⲁⲓ ⲛⲁⲛ Ⲫ̀ⲛⲟⲩϯ Ⲡⲉⲛⲥⲱⲧⲏⲣ: ⲫⲏⲉ̀ⲧⲁⲩⲁϣϥ ⲉ̀Ⲡⲓⲥ̀ⲧⲁⲩⲣⲟⲥ: ⲉⲕⲉ̀ϧⲟⲙϧⲉⲙ ⲙ̀Ⲡ̀ⲥⲁⲧⲁⲛⲁⲥ: ⲥⲁⲡⲉⲥⲏⲧ ⲛ̀ⲛⲉⲛϭⲁⲗⲁⲩϫ.")]
    [InlineData("Ⲛⲓϣⲉⲡϩ̀ⲙⲟⲧ ⲛ̀ⲧⲉ ⲛⲏⲉ̀ⲧⲁⲩⲉⲣⲡ̀ⲣⲟⲥⲫⲉⲣⲓⲛ: ⲉ̀ⲟⲩⲧⲁⲓⲟ ⲛⲉⲙ ⲟⲩⲱ̀ⲟⲩ ⲙ̀ⲡⲉⲕⲣⲁⲛ ⲉⲑⲟⲩⲁⲃ.\r\n" + 
                "Ϣⲟⲡⲟⲩ ⲉ̀ⲣⲟⲕ ⲉ̀ϫⲉⲛ ⲡⲉⲕⲑⲩⲥⲓⲁⲧⲏⲣⲓⲟⲛ ⲉⲑⲟⲩⲁⲃ ⲛ̀ⲉⲗⲗⲟⲅⲓⲙⲟⲛ ⲛ̀ⲧⲉ ⲧ̀ⲫⲉ: ⲉ̀ⲟⲩⲥ̀ⲑⲟⲓ ⲛ̀ⲥ̀ⲑⲟⲓⲛⲟⲩϥⲓ: ⲉ̀ϧⲟⲩⲛ ⲉ̀ⲧⲉⲕⲙⲉⲧⲛⲓϣϯ ⲉⲧϧⲉⲛ ⲛⲓⲫⲏⲟⲩⲓ̀: ⲉ̀ⲃⲟⲗϩⲓⲧⲉⲛ ⲡ̀ϣⲉⲙϣⲓ ⲛ̀ⲧⲉ ⲛⲉⲕⲁⲅⲅⲉⲗⲟⲥ ⲛⲉⲙ ⲛⲉⲕⲁⲣⲭⲏⲁⲅⲅⲉⲗⲟⲥ ⲉⲑⲟⲩⲁⲃ. Ⲙ̀ⲫ̀ⲣⲏϯ ⲉ̀ⲧⲁⲕϣⲱⲡ ⲉ̀ⲣⲟⲕ ⲛ̀ⲛⲓⲇⲱⲣⲟⲛ ⲛ̀ⲧⲉ ⲡⲓⲑ̀ⲙⲏⲓ Ⲁ̀ⲃⲉⲗ: ⲛⲉⲙ ϯⲑⲩⲥⲓⲁ ⲛ̀ⲧⲉ ⲡⲉⲛⲓⲱⲧ Ⲁⲃⲣⲁⲁⲙ ⲛⲉⲙ ϯⲧⲉⲃⲓ ⲥ̀ⲛⲟⲩϯ ⲛ̀ⲧⲉ ϯⲭⲏⲣⲁ.\r\n" + 
                "Ⲡⲁⲓⲣⲏϯ ⲟⲛ ⲛⲓⲕⲉⲉⲩⲭⲁⲣⲓⲥⲧⲏⲣⲓⲟⲛ: ϣⲟⲡⲟⲩ ⲉ̀ⲣⲟⲕ: ⲛⲁ ⲡⲓϩⲟⲩⲟ̀ ⲛⲉⲙ ⲛⲁ ⲡⲓⲕⲟⲩϫⲓ: ⲛⲏⲉⲧϩⲏⲡ ⲛⲉⲙ ⲛⲏⲉⲑⲟⲩⲱⲛϩ ⲉ̀ⲃⲟⲗ.\r\n" + 
                "Ⲛⲏⲉⲑⲟⲩⲱϣ ⲉ̀ⲓ̀ⲛⲓ ⲛⲁⲕ ⲉ̀ϧⲟⲩⲛ ⲟⲩⲟϩ ⲙ̀ⲙⲟⲛ ⲛ̀ⲧⲱⲟⲩ: ⲛⲉⲙ ⲛⲏⲉ̀ⲧⲁⲩⲓ̀ⲛⲓ ⲛⲁⲕ ⲉ̀ϧⲟⲩⲛ ϧⲉⲛ ⲡⲁⲓⲉ̀ϩⲟⲟⲩ ⲛ̀ⲧⲉ ⲫⲟⲟⲩ ⲛ̀ⲛⲁⲓⲇⲱⲣⲟⲛ ⲛⲁⲓ. Ⲙⲟⲓ ⲛⲱⲟⲩ ⲛ̀ⲛⲓⲁⲧⲧⲁⲕⲟ ⲛ̀ⲧ̀ϣⲉⲃⲓⲱ ⲛ̀ⲛⲏⲉⲑⲛⲁⲧⲁⲕⲟ: ⲛⲁ ⲛⲓⲫⲏⲟⲩⲓ̀ ⲛ̀ⲧ̀ϣⲉⲃⲓⲱ ⲛ̀ⲛⲁⲡ̀ⲕⲁϩⲓ: ⲛⲓϣⲁⲉ̀ⲛⲉϩ ⲛ̀ⲧ̀ϣⲉⲃⲓⲱ ⲛ̀ⲛⲓⲡ̀ⲣⲟⲥⲟⲩⲥⲏⲟⲩ. Ⲛⲟⲩⲏ̀ⲟⲩ ⲛⲟⲩⲧⲁⲙⲓⲟⲛ ⲙⲁϩⲟⲩ ⲉ̀ⲃⲟⲗ ϧⲉⲛ ⲁ̀ⲅⲁⲑⲟⲛ ⲛⲓⲃⲉⲛ.\r\n" + 
                "Ⲙⲁⲧⲁⲕⲧⲟ ⲉ̀ⲣⲱⲟⲩ Ⲡϭⲟⲓⲥ ⲛ̀ⲧ̀ϫⲟⲙ ⲛ̀ⲧⲉ ⲛⲉⲕⲁⲅⲅⲉⲗⲟⲥ ⲛⲉⲙ ⲛⲉⲕⲁⲣⲭⲏⲁⲅⲅⲉⲗⲟⲥ ⲉⲑⲟⲩⲁⲃ.\r\n" + 
                "Ⲙ̀ⲫ̀ⲣⲏϯ ⲉ̀ⲧⲁⲩⲉⲣⲫ̀ⲙⲉⲩⲓ̀ ⲙ̀ⲡⲉⲕⲣⲁⲛ ⲉⲑⲟⲩⲁⲃ ϩⲓϫⲉⲛ ⲡⲓⲕⲁϩⲓ: ⲁ̀ⲣⲓⲡⲟⲩⲙⲉⲩⲓ̀ ϩⲱⲟⲩ Ⲡϭⲟⲓⲥ ϧⲉⲛ ⲧⲉⲕⲙⲉⲧⲟⲩⲣⲟ: ⲟⲩⲟϩ ϧⲉⲛ ⲡⲁⲓ ⲕⲉ ⲉ̀ⲱⲛ ⲫⲁⲓ ⲙ̀ⲡⲉⲣⲭⲁⲩ ⲛ̀ⲥⲱⲕ. ")]
    public async Task AnnotateBohairicCoptic(string text)
    {
        var sentence = _translator.AnnotateAsync(text);

        List<IStructuralElement> demo = [];

        await foreach (var word in sentence)
        {
            bool isFirstInterpretation = true;
            await foreach (var wordInterpretation in word)
            {
                //var text = string.Join("⸱", element.Select(y => noun.Substring(y.SourceRange)));
                var annotations = string.Join(", ", wordInterpretation.Select(y => y.ToString()));

                //_output.WriteLine(text);
                _output.WriteLine(annotations);

                if (isFirstInterpretation)
                {
                    demo.AddRange(wordInterpretation);
                    isFirstInterpretation = false;
                }
            }
            _output.WriteLine("");
        }

        var demoText = DemoText(demo);
        _output.WriteLine(demoText);
    }

    private static string DemoText(IEnumerable<IStructuralElement> elements)
    {
        return string.Join(" ", elements.Select(y => y switch
        {
            DeterminerElement detElem => detElem.Meta switch
            {
                DeterminerArticleMeta articleMeta => articleMeta switch
                {
                    { Definite: false } => articleMeta.Target.Number == GrammaticalCount.Singular ? "a" : "",
                    _ => articleMeta.Strength == DeterminerStrength.Weak ? "The" : "the",
                },
                DeterminerPossessiveMeta possessiveMeta => possessiveMeta.Possessor switch
                {
                    { PointOfView: PointOfView.First }
                        => possessiveMeta.Possessor.Number == GrammaticalCount.Singular ? "my" : "our",
                    { PointOfView: PointOfView.Second }
                        => possessiveMeta.Possessor.Number == GrammaticalCount.Singular ? "your" : "y'all's",
                    { PointOfView: PointOfView.Third, Number: GrammaticalCount.Singular }
                        => possessiveMeta.Possessor.Gender == Gender.Masculine ? "his" : "her",
                    { PointOfView: PointOfView.Third, Number: GrammaticalCount.Plural } => "their",
                    _ => "of"   // not really correct but whatever
                },
                DeterminerDemonstrativeMeta => "this",
            },

            PrepositionElement prepElem => (prepElem.Meta.Negative ? "not " : "")
                + prepElem.Meta.Type.ToString().ToLower(),

            StructuralLexeme lexElem => lexElem.Sense.Translations.GetByLanguage(CoptLib.Writing.KnownLanguage.English).ToString(),
            LexemeElement nounElem => nounElem.Meta.Meaning.Orthography,
        }));
    }
}
