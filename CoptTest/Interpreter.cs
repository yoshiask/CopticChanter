using CoptLib.Writing;
using CoptLib.Writing.Linguistics;
using CoptLib.Writing.Linguistics.Analyzers;
using System.Collections.Generic;
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

        public static readonly string[] IpaTranscribe_CopticUnicode_Samples = new[]
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

        public static readonly string[] IpaTranscribe_CopticStandard_Samples = new[]
        {
            "}polic `m'u,on",
            "Taisoury `nnoub `nka;aroc etfai qa pi`arwmata@ etqen nenjij `n`Aarwn pi`ouyb eftale ou`c;oinoufi `e`pswi `ejen pima `n`ersw`ousi.",
        };

        public static readonly IEnumerable<object[]> Transliterate_CopticUnicode_Samples = new List<object[]>
        {
            new object[] { "ⲛ̀Ⲇⲁⲩⲓⲇ", ".enDavid" },
            new object[] { "ⲁⲣⲭⲱⲛ", "arkʰon" },
            new object[] { "ⲡ̀ⲁⲣⲭⲱⲛ", ".eparkʰon" },
            new object[] { "ⲙ̀ⲡ̀ⲣⲉⲥⲃⲩⲧⲉⲣⲟⲥ", ".em.epresviteros" },
            new object[] { "ⲛ̀ⲁ̀ⲣⲭⲏⲁⲅⲅⲉⲗⲟⲥ", ".en.arsʰiangelos"},
            new object[] { "ⲛ̀ⲁ̀ⲣⲭⲏⲉ̀ⲣⲉⲩⲥ", ".en.arsʰi.erevs"},
            new object[] { "ⲛⲓⲉⲩⲭⲏ", "nievsʰi"},
            new object[] { "ⲉⲩⲭⲏ", "evsʰi"},
            new object[] { "ⲡ̀ⲯⲩⲭⲏ", ".eppsisʰi"},
            new object[] { "ⲯⲩⲭⲏ", "psisʰi"},
            new object[] { "ⲭⲱ", "ko"},
            new object[] { "ⲙ̀ⲡⲓⲭⲱ", ".empiko"},
            new object[] { "ⲭⲱⲣⲓⲥ", "kʰoris"},
            new object[] { "Ⲭⲉⲣⲉ ⲛⲉ Ⲙⲁⲣⲓⲁ", "Sʰere ne Maria" },
            new object[] { "Ⲱⲥⲁⲛⲛⲁ", "Osanna" },
            new object[] { "Ⲡⲓⲭ̀ⲣⲓⲥⲧⲟⲥ", "Pi.ekʰristos" },
            new object[] { "Ⲭⲣⲓⲥⲧⲟⲥ", "Kʰristos" },
            new object[] { "ⲛⲓⲁ̀ⲅⲅⲉⲗⲟⲥ", "ni.angelos" },
            new object[] { "ⲛⲓⲁⲅⲅⲉⲗⲟⲥ", "niangelos" },
            new object[] { "ⲓⲣⲏⲛⲏ", "irini" },
            new object[] { "Ⲓⲥⲭⲩⲣⲟⲛ", "Iskiron" },
            new object[] { "Ⲟⲩⲟϩ", "Owoh" },
            new object[] { "ⲁϥϭⲓⲥⲁⲣⲝ", "afcʰisarex" },
            new object[] { "ⲛ̀ⲛⲓⲡⲁⲭⲛⲏ", ".ennipakʰni" },
            new object[] { "Ⲧⲉⲛⲟⲩⲱϣⲧ ⲙ̀ⲙⲟⲕ ⲱ̀ Ⲡⲓⲭ̀ⲣⲓⲥⲧⲟⲥ: ⲛⲉⲙ Ⲡⲉⲕⲓⲱⲧ ⲛ̀ⲁ̀ⲅⲁⲑⲟⲥ: ⲛⲉⲙ Ⲡⲓⲡ̀ⲛⲉⲩⲙⲁ ⲉⲑⲟⲩⲁⲃ: ϫⲉ ⲁⲕⲓ̀ ⲁⲕⲥⲱϯ ⲙ̀ⲙⲟⲛ.",
                           "Tenuosʰt .emmok .o Pi.ekʰristos: nem Pekyot .en.agʰatʰos: nem Pi.epnevma etʰowab: je ak.i aksoti .emmon." },
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
            string result = _analyzer.IpaTranscribe(CopticFont.CsAvvaShenouda.Convert(sample));
            _output.WriteLine(result);
        }

        [Theory]
        [MemberData(nameof(Transliterate_CopticUnicode_Samples))]
        public void Transliterate_CopticUnicode(string sample, string expected)
        {
            var result = _analyzer.Transliterate(sample, KnownLanguage.English);
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

        public static IEnumerable<object[]> GetIpaTranscribe_CopticUnicode_Samples()
        {
            foreach (string sample in IpaTranscribe_CopticUnicode_Samples)
                yield return new object[] { sample };
        }

        public static IEnumerable<object[]> GetIpaTranscribe_CopticStandard_Samples()
        {
            foreach (string sample in IpaTranscribe_CopticStandard_Samples)
                yield return new object[] { sample };
        }
    }
}
