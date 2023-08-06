using CoptLib.Writing;
using CoptLib.Writing.Linguistics;
using CoptLib.Writing.Linguistics.Analyzers;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace CoptTest
{
    public class Interpreter
    {
        private readonly ITestOutputHelper _output;
        private readonly LinguisticAnalyzer _analyzer = new CopticGrecoBohairicAnalyzer();

        public Interpreter(ITestOutputHelper output)
        {
            _output = output;
        }

        public static readonly string[] IpaTranscribe_CopticUnicode_Samples =
        {
            // Difficult words
            "ⲙ̀ⲡ̀ⲣⲉⲥⲃⲩⲧⲉⲣⲟⲥ", "Ⲱⲥⲁⲛⲛⲁ", "Ⲡⲓⲭ̀ⲣⲓⲥⲧⲟⲥ", "Ⲭⲣⲓⲥⲧⲟⲥ", "ⲛⲓⲁ̀ⲅⲅⲉⲗⲟⲥ", "ⲓⲣⲏⲛⲏ", "ⲟⲩⲟϩ",

            // Ressurrection hymns
            "Ⲡⲓⲭ̀ⲣⲓⲥⲧⲟⲥ ⲁϥⲧⲱⲛϥ ⲉ̀ⲃⲟⲗ ϧⲉⲛ ⲛⲏⲉⲑⲙⲱⲟⲩⲧ: ⲫⲏⲉ̀ⲧⲁϥⲙⲟⲩ ⲁϥϩⲱⲙⲓ ⲉ̀ϫⲉⲛ ⲫ̀ⲙⲟⲩ ⲟⲩⲟϩ ⲛⲏⲉⲧⲭⲏ ϧⲉⲛ ⲛⲓⲙ̀ϩⲁⲩ ⲁϥⲉⲣϩ̀ⲙⲟⲧ ⲛⲱⲟⲩ ⲙ̀ⲡⲓⲱⲛϧ ⲛ̀ⲉ̀ⲛⲉϩ.",
            "Ⲭⲣⲓⲥⲧⲟⲥ ⲁ̀ⲛⲉⲥⲧⲏ ⲉⲕ ⲛⲉⲕⲣⲱⲛ: ⲑⲁⲛⲁⲧⲱ ⲑⲁⲛⲁⲧⲟⲛ: ⲡⲁⲧⲏⲥⲁⲥ ⲕⲉ ⲧⲓⲥ ⲉⲛ ⲧⲓⲥ ⲙ̀ⲛⲏⲙⲁⲥⲓ ⲍⲱⲏⲛ ⲭⲁⲣⲓⲥⲁⲙⲉⲛⲟⲥ.",

            // Morning Praises
            "Ⲧⲉⲛⲟ̀ⲩⲱ̀ϣⲧ ⲙ̀Ⲫ̀ⲓⲱⲧ ⲛⲉⲙ ⲡ̀Ϣⲏⲣⲓ: ⲛⲉⲙ ⲡⲓⲠ̀ⲛⲉⲩⲙⲁ ⲉⲑⲟⲩⲁⲃ: ⲭⲉⲣⲉ ϯⲉ̀ⲕⲕⲗⲏⲥⲓⲁ: ⲡ̀ⲏⲓ ⲛ̀ⲧⲉ ⲛⲓⲁ̀ⲅⲅⲉⲗⲟⲥ",

            // First Hoos Lobsh
            "Ϧⲉⲛ ⲟⲩϣⲱⲧ ⲁϥϣⲱⲧ: ⲛ̀ϫⲉ ⲡⲓⲙⲱⲟⲩ ⲛ̀ⲧⲉ ⲫ̀ⲓⲟⲙ: ⲟⲩⲟϩ ⲫ̀ⲛⲟⲩⲛ ⲉⲧϣⲏⲕ: ⲁϥϣⲱⲡⲓ ⲛ̀ⲟⲩⲙⲁ ⲙ̀ⲙⲟϣⲓ.",
            "Ⲟⲩⲕⲁϩⲓ ⲛ̀ⲁⲑⲟⲩⲱⲛϩ: ⲁ̀ⲫ̀ⲣⲏ ϣⲁⲓ ϩⲓϫⲱϥ: ⲟⲩⲙⲱⲓⲧ ⲛ̀ⲁⲧⲥⲓⲛⲓ: ⲁⲩⲙⲟϣⲓ ϩⲓⲱⲧϥ.",
            "Ⲟⲩⲙⲱⲟⲩ ⲉϥⲃⲏⲗ ⲉ̀ⲃⲟⲗ: ⲁϥⲟ̀ϩⲓ ⲉ̀ⲣⲁⲧϥ: ϧⲉⲛ ⲟⲩϩⲱⲃ ⲛ̀ϣ̀ⲫⲏⲣⲓ: ⲙ̀ⲡⲁⲣⲁⲇⲟⲝⲟⲛ.",
            "Ⲫⲁⲣⲁⲱ̀ ⲛⲉⲙ ⲛⲉϥϩⲁⲣⲙⲁ: ⲁⲩⲱⲙⲥ ⲉ̀ⲡⲉⲥⲏⲧ: ⲛⲉⲛϣⲏⲣⲓ ⲙ̀Ⲡⲓⲥⲣⲁⲏⲗ: ⲁⲩⲉⲣϫⲓⲛⲓⲟⲣ ⲙ̀ⲫ̀ⲓⲟⲙ.",
            "Ⲉ̀ⲛⲁϥϩⲱⲥ ϧⲁϫⲱⲟⲩ ⲡⲉ: ⲛ̀ϫⲉ Ⲙⲱⲩ̀ⲥⲏⲥ ⲡⲓⲡ̀ⲣⲟⲫⲏⲧⲏⲥ: ϣⲁ ⲛ̀ⲧⲉϥϭⲓⲧⲟⲩ ⲉ̀ϧⲟⲩⲛ: ϩⲓ ⲡ̀ϣⲁϥⲉ ⲛ̀Ⲥⲓⲛⲁ.",
            "Ⲉ̀ⲛⲁϥϩⲱⲥ ⲉ̀Ⲫ̀ⲛⲟⲩϯ: ϧⲉⲛ ⲧⲁⲓϩⲱⲇⲏ ⲙ̀ⲃⲉⲣⲓ: ϫⲉ ⲙⲁⲣⲉⲛϩⲱⲥ ⲉ̀Ⲡ̀ϭⲟⲓⲥ: ϫⲉ ϧⲉⲛ ⲟⲩⲱ̀ⲟⲩ ⲅⲁⲣ ⲁϥϭⲓⲱ̀ⲟⲩ.",
            "Ϩⲓⲧⲉⲛ ⲛⲓⲉⲩⲭⲏ: ⲛ̀ⲧⲉ Ⲙⲱⲩ̀ⲥⲏⲥ ⲡⲓⲁⲣⲭⲏⲡ̀ⲣⲟⲫⲏⲧⲏⲥ: Ⲡ̀ϭⲟⲓⲥ ⲁ̀ⲣⲓϩ̀ⲙⲟⲧ ⲛⲁⲛ: ⲙ̀ⲡⲓⲭⲱ ⲉ̀ⲃⲟⲗ ⲛ̀ⲧⲉ ⲛⲉⲛⲛⲟⲃⲓ.",
            "Ϩⲓⲧⲉⲛ ⲛⲓⲡ̀ⲣⲉⲥⲃⲓⲁ: ⲛ̀ⲧⲉ Ϯⲑⲉⲟ̀ⲧⲟⲕⲟⲥ ⲉⲑⲟⲩⲁⲃ Ⲙⲁⲣⲓⲁ: Ⲡ̀ϭⲟⲓⲥ ⲁ̀ⲣⲓϩ̀ⲙⲟⲧ ⲛⲁⲛ: ⲙ̀ⲡⲓⲭⲱ ⲉ̀ⲃⲟⲗ ⲛ̀ⲧⲉ ⲛⲉⲛⲛⲟⲃⲓ.",
            "Ⲧⲉⲛⲟⲩⲱϣⲧ ⲙ̀ⲙⲟⲕ ⲱ̀ Ⲡⲓⲭ̀ⲣⲓⲥⲧⲟⲥ: ⲛⲉⲙ Ⲡⲉⲕⲓⲱⲧ ⲛ̀ⲁ̀ⲅⲁⲑⲟⲥ: ⲛⲉⲙ Ⲡⲓⲡ̀ⲛⲉⲩⲙⲁ ⲉⲑⲟⲩⲁⲃ: ϫⲉ ⲁⲕⲓ̀ ⲁⲕⲥⲱϯ ⲙ̀ⲙⲟⲛ.",
        };

        public static readonly string[] IpaTranscribe_CopticStandard_Samples =
        {
            "}polic `m'u,on",
            "Taisoury `nnoub `nka;aroc etfai qa pi`arwmata@ etqen nenjij `n`Aarwn pi`ouyb eftale ou`c;oinoufi `e`pswi `ejen pima `n`ersw`ousi.",
        };

        public static readonly IEnumerable<object[]> Transliterate_CopticUnicode_Samples = new List<object[]>
        {
            new object[] { "ⲛ̀Ⲇⲁⲩⲓⲇ", "en·Dă·vid" },
            new object[] { "ⲁⲣⲭⲱⲛ", "ăr·kʰon" },
            new object[] { "ⲡ̀ⲁⲣⲭⲱⲛ", "ep·ăr·kʰon" },
            new object[] { "ⲙ̀ⲡ̀ⲣⲉⲥⲃⲩⲧⲉⲣⲟⲥ", "em·ep·res·vi·te·ros" },
            new object[] { "ⲛ̀ⲁ̀ⲣⲭⲏⲁⲅⲅⲉⲗⲟⲥ", "en·ăr·sʰi·ăn·ge·los" },
            new object[] { "ⲛ̀ⲁ̀ⲣⲭⲏⲉ̀ⲣⲉⲩⲥ", "en·ăr·sʰi·e·revs" },
            new object[] { "ⲛⲓⲉⲩⲭⲏ", "ni·ev·sʰi" },
            new object[] { "ⲉⲩⲭⲏ", "ev·sʰi" },
            new object[] { "ⲡ̀ⲯⲩⲭⲏ", "ep·psi·sʰi" },
            new object[] { "ⲯⲩⲭⲏ", "psi·sʰi" },
            new object[] { "ⲭⲱ", "ko" },
            new object[] { "ⲙ̀ⲡⲓⲭⲱ", "em·pĭ·ko" },
            new object[] { "ⲭⲱⲣⲓⲥ", "kʰo·rĭs" },
            new object[] { "Ⲭⲉⲣⲉ ⲛⲉ Ⲙⲁⲣⲓⲁ", "Sʰe·re ne Mă·rĭ·ă" },
            new object[] { "Ⲱⲥⲁⲛⲛⲁ", "O·săn·nă" },
            new object[] { "Ⲡⲓⲭ̀ⲣⲓⲥⲧⲟⲥ", "Pi·ekʰ·rĭs·tos" },
            new object[] { "Ⲭⲣⲓⲥⲧⲟⲥ", "Kʰrĭs·tos" },
            new object[] { "ⲛⲓⲁ̀ⲅⲅⲉⲗⲟⲥ", "ni·ăn·ge·los" },
            new object[] { "ⲛⲓⲁⲅⲅⲉⲗⲟⲥ", "ni·ăn·ge·los" },
            new object[] { "ⲓⲣⲏⲛⲏ", "ĭ·ri·ni" },
            new object[] { "Ⲓⲥⲭⲩⲣⲟⲛ", "Ĭs·kĭ·ron" },
            new object[] { "Ⲟⲩⲟϩ", "O·woh" },
            new object[] { "ⲁϥϭⲓⲥⲁⲣⲝ", "ăf·cʰĭ·sărx" },
            new object[] { "ⲛ̀ⲛⲓⲡⲁⲭⲛⲏ", "en·ni·păkʰ·ni" },
            new object[] { "Ⲟⲩⲕⲁϩⲓ", "U·kă·hi" },
            new object[] { "ⲉⲑⲟⲩⲁⲃ", "etʰ·o·wab" },
            new object[] { "Ⲧⲉⲛⲟⲩⲱϣⲧ", "Ten·u·osʰt" },
            new object[] { "ⲁⲕⲥⲱϯ", "ăk·so·ti" },
            new object[] { "ⲉ̀ⲧⲉⲛⲥ̀ⲙⲏ", "e·ten·es·mi" },
            new object[] { "Ⲡⲉⲛⲛⲟⲩϯ", "Pen·nu·ti" },
            new object[] { "ⲙ̀Ⲫ̀ⲓⲱⲧ", "em·Ef·yot" },
            new object[] { "ⲡⲉⲧⲥ̀ϣⲉ", "pet·es·sʰe" },
            new object[] { "ⲛⲁϩⲙⲉⲛ", "năh·men" },
            new object[] { "\"Ⲡⲓⲱⲓⲕ\"", "\"Pi·ɔik\"" },
            new object[] { "Ⲧⲉⲛⲟⲩⲱϣⲧ ⲙ̀ⲙⲟⲕ ⲱ̀ Ⲡⲓⲭ̀ⲣⲓⲥⲧⲟⲥ: ⲛⲉⲙ Ⲡⲉⲕⲓⲱⲧ ⲛ̀ⲁ̀ⲅⲁⲑⲟⲥ: ⲛⲉⲙ Ⲡⲓⲡ̀ⲛⲉⲩⲙⲁ ⲉⲑⲟⲩⲁⲃ: ϫⲉ ⲁⲕⲓ̀ ⲁⲕⲥⲱϯ ⲙ̀ⲙⲟⲛ.",
                           "Ten·u·osʰt em·mok o Pi·ekʰ·rĭs·tos: nem Pek·yot en·ă·gʰă·tʰos: nem Pi·ep·nev·mă etʰ·o·wab: je ăk·i ăk·so·ti em·mon." },
        };

        [Theory]
        [MemberData(nameof(GetIpaTranscribe_CopticUnicode_Samples))]
        public void IpaTranscribe_CopticUnicode(string sample)
        {
            var result = _analyzer.IpaTranscribe(sample);
            _output.WriteLine(result);
        }

        [Theory]
        [MemberData(nameof(GetIpaTranscribe_CopticStandard_Samples))]
        public void IpaTranscribe_CopticStandard(string sample)
        {
            string result = _analyzer.IpaTranscribe(DisplayFont.CopticStandard.Convert(sample));
            _output.WriteLine(result);
        }

        [Theory]
        [MemberData(nameof(Transliterate_CopticUnicode_Samples))]
        public void Transliterate_CopticUnicode(string sample, string expected)
        {
            var result = _analyzer.Transliterate(sample, KnownLanguage.English, PhoneticWord.DEFAULT_SYLLABLE_SEPARATOR);
            _output.WriteLine(result);
            Assert.Equal(expected, result);
        }

        CopticOldBohairicAnalyzer _obAnalyzer = new();
        [Theory]
        [MemberData(nameof(GetIpaTranscribe_CopticUnicode_Samples))]
        public void IpaTranscribe_OldBohairic(string sample)
        {
            var result = _obAnalyzer.IpaTranscribe(sample);
            _output.WriteLine(result);
        }

        private GreekAnalyzer _elAnalyzer = new();
        [Theory]
        [InlineData("σταυρος", "stăv·ros")]
        public void Transliterate_Greek(string sample, string expected)
        {
            var result = _elAnalyzer.Transliterate(sample, KnownLanguage.English, PhoneticWord.DEFAULT_SYLLABLE_SEPARATOR);
            _output.WriteLine(result);
            Assert.Equal(expected, result);
        }

        public static IEnumerable<object[]> GetIpaTranscribe_CopticUnicode_Samples()
            => IpaTranscribe_CopticUnicode_Samples.Select(sample => new object[] { sample });

        public static IEnumerable<object[]> GetIpaTranscribe_CopticStandard_Samples()
            => IpaTranscribe_CopticStandard_Samples.Select(sample => new object[] { sample });
    }
}
