using CoptLib.Writing;
using CoptLib.Writing.Linguistics;
using CoptLib.Writing.Linguistics.Analyzers;
using Xunit;
using Xunit.Abstractions;

namespace CoptTest
{
    public class Interpreter(ITestOutputHelper output)
    {
        private readonly LinguisticAnalyzer _analyzer = new CopticGrecoBohairicAnalyzer();
        public static readonly TheoryData<string> IpaTranscribe_CopticUnicode_Samples = new()
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

        public static readonly TheoryData<string> IpaTranscribe_CopticStandard_Samples = new()
        {
            "}polic `m'u,on",
            "Taisoury `nnoub `nka;aroc etfai qa pi`arwmata@ etqen nenjij `n`Aarwn pi`ouyb eftale ou`c;oinoufi `e`pswi `ejen pima `n`ersw`ousi.",
        };

        public static readonly TheoryData<string, string> Transliterate_CopticUnicode_Samples = new()
        {
            { "ⲛ̀Ⲇⲁⲩⲓⲇ", "en·Dă·vid" },
            { "ⲁⲣⲭⲱⲛ", "ăr·kʰon" },
            { "ⲡ̀ⲁⲣⲭⲱⲛ", "ep·ăr·kʰon" },
            { "ⲙ̀ⲡ̀ⲣⲉⲥⲃⲩⲧⲉⲣⲟⲥ", "em·ep·res·vi·te·ros" },
            { "ⲛ̀ⲁ̀ⲣⲭⲏⲁⲅⲅⲉⲗⲟⲥ", "en·ăr·sʰi·ăn·ge·los" },
            { "ⲛ̀ⲁ̀ⲣⲭⲏⲉ̀ⲣⲉⲩⲥ", "en·ăr·sʰi·e·revs" },
            { "ⲛⲓⲉⲩⲭⲏ", "ni·ev·sʰi" },
            { "ⲉⲩⲭⲏ", "ev·sʰi" },
            { "ⲡ̀ⲯⲩⲭⲏ", "ep·psi·sʰi" },
            { "ⲯⲩⲭⲏ", "psi·sʰi" },
            { "ⲭⲱ", "ko" },
            { "ⲙ̀ⲡⲓⲭⲱ", "em·pĭ·ko" },
            { "ⲭⲱⲣⲓⲥ", "kʰo·rĭs" },
            { "Ⲭⲉⲣⲉ ⲛⲉ Ⲙⲁⲣⲓⲁ", "Sʰe·re ne Mă·rĭ·ă" },
            { "Ⲱⲥⲁⲛⲛⲁ", "O·săn·nă" },
            { "Ⲡⲓⲭ̀ⲣⲓⲥⲧⲟⲥ", "Pi·ekʰ·rĭs·tos" },
            { "Ⲭⲣⲓⲥⲧⲟⲥ", "Kʰrĭs·tos" },
            { "ⲛⲓⲁ̀ⲅⲅⲉⲗⲟⲥ", "ni·ăn·ge·los" },
            { "ⲛⲓⲁⲅⲅⲉⲗⲟⲥ", "ni·ăn·ge·los" },
            { "ⲓⲣⲏⲛⲏ", "ĭ·ri·ni" },
            { "Ⲓⲥⲭⲩⲣⲟⲛ", "Ĭs·kĭ·ron" },
            { "Ⲟⲩⲟϩ", "O·woh" },
            { "ⲁϥϭⲓⲥⲁⲣⲝ", "ăf·cʰĭ·sărx" },
            { "ⲛ̀ⲛⲓⲡⲁⲭⲛⲏ", "en·ni·păkʰ·ni" },
            { "Ⲟⲩⲕⲁϩⲓ", "U·kă·hi" },
            { "ⲉⲑⲟⲩⲁⲃ", "etʰ·o·wab" },
            { "Ⲧⲉⲛⲟⲩⲱϣⲧ", "Ten·u·osʰt" },
            { "ⲁⲕⲥⲱϯ", "ăk·so·ti" },
            { "ⲉ̀ⲧⲉⲛⲥ̀ⲙⲏ", "e·ten·es·mi" },
            { "Ⲡⲉⲛⲛⲟⲩϯ", "Pen·nu·ti" },
            { "ⲙ̀Ⲫ̀ⲓⲱⲧ", "em·Ef·yot" },
            { "ⲡⲉⲧⲥ̀ϣⲉ", "pet·es·sʰe" },
            { "ⲛⲁϩⲙⲉⲛ", "năh·men" },
            { "\"Ⲡⲓⲱⲓⲕ\"", "\"Pi·ɔik\"" },
            { "Ⲧⲉⲛⲟⲩⲱϣⲧ ⲙ̀ⲙⲟⲕ ⲱ̀ Ⲡⲓⲭ̀ⲣⲓⲥⲧⲟⲥ: ⲛⲉⲙ Ⲡⲉⲕⲓⲱⲧ ⲛ̀ⲁ̀ⲅⲁⲑⲟⲥ: ⲛⲉⲙ Ⲡⲓⲡ̀ⲛⲉⲩⲙⲁ ⲉⲑⲟⲩⲁⲃ: ϫⲉ ⲁⲕⲓ̀ ⲁⲕⲥⲱϯ ⲙ̀ⲙⲟⲛ.",
                           "Ten·u·osʰt em·mok o Pi·ekʰ·rĭs·tos: nem Pek·yot en·ă·gʰă·tʰos: nem Pi·ep·nev·mă etʰ·o·wab: je ăk·i ăk·so·ti em·mon." },
        };

        [Theory]
        [MemberData(nameof(IpaTranscribe_CopticUnicode_Samples))]
        public void IpaTranscribe_CopticUnicode(string sample)
        {
            var result = _analyzer.IpaTranscribe(sample);
            output.WriteLine(result);
        }

        [Theory]
        [MemberData(nameof(IpaTranscribe_CopticStandard_Samples))]
        public void IpaTranscribe_CopticStandard(string sample)
        {
            string result = _analyzer.IpaTranscribe(DisplayFont.CopticStandard.Convert(sample));
            output.WriteLine(result);
        }

        [Theory]
        [MemberData(nameof(Transliterate_CopticUnicode_Samples))]
        public void Transliterate_CopticUnicode(string sample, string expected)
        {
            var result = _analyzer.Transliterate(sample, KnownLanguage.English);
            output.WriteLine(result);
            Assert.Equal(expected, result);
        }

        CopticOldBohairicAnalyzer _obAnalyzer = new();
        [Theory]
        [MemberData(nameof(IpaTranscribe_CopticUnicode_Samples))]
        public void IpaTranscribe_OldBohairic(string sample)
        {
            var result = _obAnalyzer.IpaTranscribe(sample);
            output.WriteLine(result);
        }

        private GreekAnalyzer _elAnalyzer = new();
        [Theory]
        [InlineData("σταυρος", "stăv·ros")]
        public void Transliterate_Greek(string sample, string expected)
        {
            var result = _elAnalyzer.Transliterate(sample, KnownLanguage.English);
            output.WriteLine(result);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("Holy God, Holy Mighty, Holy Immortal, have mercy on us.", "en")]
        [InlineData("Ἅγιος ὁ Θεός, ἅγιος Ἰσχυρός, ἅγιος Ἀθάνατος, ἐλέησον ἡμᾶς.", "el")]
        [InlineData("Ⲁⲅⲓⲟⲥ ⲟ Ⲑⲉⲟⲥ: ⲁⲅⲓⲟⲥ ⲓⲥⲭⲩⲣⲟⲥ: ⲁⲅⲓⲟⲥ ⲁⲑⲁⲛⲁⲧⲟⲥ: ⲉⲗⲉⲏⲥⲟⲛ ⲏⲙⲁⲥ.", "cop")]
        [InlineData("Ⲁ̀ⲅⲓⲟⲥ ⲟ̀ Ⲑⲉⲟⲥ: ⲁ̀ⲅⲓⲟⲥ Ⲓⲥⲭⲩⲣⲟⲥ: ⲁ̀ⲅⲓⲟⲥ Ⲁ̀ⲑⲁⲛⲁⲧⲟⲥ: ⲉ̀ⲗⲉⲏ̀ⲥⲟⲛ ⲏ̀ⲙⲁⲥ.", "cop")]
        [InlineData("Ⲡ̀ϭⲟⲓⲥ ⲭⲁ ⲛⲉⲛⲛⲟⲃⲓ ⲛⲁⲛ ⲉ̀ⲃⲟⲗ. Ⲡ̀ϭⲟⲓⲥ ⲭⲁ ⲛⲉⲛⲁ̀ⲛⲟⲙⲓⲁ ⲛⲁⲛ ⲉ̀ⲃⲟⲗ.", "cop")]
        [InlineData("Πϭοις χα νεννοβι ναν εβολ. Πϭοις χα νενανομια ναν εβολ.", "cop")]
        [InlineData("قُدُّوسٌ الله، قُدُّوسٌ القَويُّ، قُدُّوسٌ الحَيُّ الَّذي لا يَموتُ، إرْحَمْنا.", "ar")]
        public void IdentifyLanguage(string sample, string exTag)
        {
            var exLang = LanguageInfo.Parse(exTag);

            bool success = LinguisticLanguageService.TryIdentifyLanguage(sample, out var acLang);
            Assert.True(success);
            Assert.Equal(exLang, acLang);
        }
    }
}
