using CoptLib.Extensions;
using CoptLib.Writing.Lexicon;
using CoptLib.Writing.Linguistics;
using CoptLib.Writing.Linguistics.Analyzers;
using CoptLib.Writing.Linguistics.XBar;
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
    [InlineData("ⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified, RelativeTime.Present)]
    [InlineData("ⲙⲡⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲧⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲧⲙⲡⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲉⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲉⲙⲡⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲧⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲙⲡⲁϯⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲁϯⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲧⲙⲡⲁϯⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲁⲙⲡⲁϯⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ϣⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲙⲡⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉϣⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲧϣⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲧⲙⲡⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲁϣⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲁⲙⲡⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲉⲓⲉⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲛⲁⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲙⲁⲣⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲙⲡⲉⲛⲑⲣⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲛⲧⲁⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲧⲁⲣⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ϣⲁϯⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲁⲓϣⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]
    [InlineData("ⲁⲓⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Singular, Gender.Unspecified)]

    [InlineData("ⲧⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲧⲉⲛⲛⲁⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲛⲛⲁⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲉⲛⲛⲁⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲁⲛⲛⲁⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲁⲛⲛⲁⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲙⲡⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲙⲡⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲉⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲉⲙⲡⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲙⲡⲁⲧⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲁⲧⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲙⲡⲁⲧⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲁⲙⲡⲁⲧⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ϣⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲙⲡⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉϣⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧϣⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲧⲙⲡⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲁϣⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲁⲙⲡⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲙⲡⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲉⲛⲉⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲛⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲙⲁⲣⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲙⲡⲉⲛⲑⲣⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲛⲧⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲧⲁⲣⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ϣⲁⲧⲉⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲁⲛϣⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    [InlineData("ⲁⲛⲥⲱⲧⲉⲙ", PointOfView.First, GrammaticalCount.Plural, Gender.Unspecified)]
    public async Task BohairicCoptic_DetectVerbs(string verb, PointOfView pov, GrammaticalCount number, Gender gender, RelativeTime start = default, RelativeTime end = default, TenseFlags flags = default)
    {
        throw new System.NotImplementedException();
        //var ctor = CopticBohairicTranslator.TryIdentifyVerb(verb);
        //Guard.IsNotNull(ctor);

        //var element = ctor(Range.All);
        //Guard.IsNotNull(element);

        //Guard.IsNotNull(element.Meta);
        //Guard.IsNotNull(element.Actor);

        //_output.WriteLine(verb);
        //_output.WriteLine(element.Meta.ToString());
        //_output.WriteLine(element.Actor.ToString());

        //Guard.IsEqualTo((byte)element.Actor.PointOfView, (byte)pov);
        //Guard.IsEqualTo((int)element.Actor.Number, (int)number);
        //Guard.IsEqualTo((byte)element.Actor.Gender, (byte)gender);
    }

    [Theory]
    [InlineData("ⲛⲉⲛⲓⲟϯ")]
    [InlineData("Ⲫⲓⲱⲧ")]
    [InlineData("ⲙⲪⲓⲱⲧ")]
    [InlineData("ⲛⲪⲓⲱⲧ")]
    [InlineData("Ⲡϭⲟⲓⲥ")]
    [InlineData("ⲡⲁϭⲟⲓⲥ")]
    [InlineData("ⲧⲁϭⲟⲓⲥ")]
    [InlineData("ⲡⲓⲙⲁ")]
    [InlineData("ⲡⲓⲙⲁⲛϣⲉⲗⲉⲧ")]
    [InlineData("ⲟⲩⲣⲉϥⲉⲣⲛⲟⲃⲓ")]
    public async Task BohairicCoptic_DetectNouns(string noun)
    {
        noun = CopticBohairicTranslator.NormalizeText(noun);
        await foreach (var x in _bohairicTranslator.IdentifyNoun(noun))
        {
            var text = string.Join("⸱", x.Select(y => noun.Substring(y.SourceRange)));
            var annotations = string.Join(", ", x.Select(y => y.ToString()));

            _output.WriteLine(text);
            _output.WriteLine(annotations);
            _output.WriteLine("");
        }
    }

    [Fact]
    public async Task AnnotateBohairicCoptic()
    {
        var sentence = _translator.AnnotateAsync("ⲡ̀ⲏⲓ ⲛ̀ⲧⲉ ⲛⲓⲁ̀ⲅⲅⲉⲗⲟⲥ");

        await foreach (var word in sentence)
        {
            await foreach (var element in word)
            {
                //var text = string.Join("⸱", element.Select(y => noun.Substring(y.SourceRange)));
                var annotations = string.Join(", ", element.Select(y => y.ToString()));

                //_output.WriteLine(text);
                _output.WriteLine(annotations);
            }
            _output.WriteLine("");
        }
    }
}
