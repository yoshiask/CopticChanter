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
                },
            },
            PrepositionElement prepElem => (prepElem.Meta.Negative ? "not " : "")
                + prepElem.Meta.Type.ToString().ToLower(),

            StructuralLexeme lexElem => lexElem.Sense.Translations.GetByLanguage(CoptLib.Writing.KnownLanguage.English).ToString(),
            NounElement nounElem => nounElem.Meta.Meaning.Orthography,
        }));
    }
}
