using CoptLib.Writing;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Xunit;

namespace CoptTest
{
    public class Interpreter
    {
        static readonly string[] IpaTranscribe_CopticUnicode_Samples = new[]
        {
            // Difficult words
            "Ⲡⲓⲭ̀ⲣⲓⲥⲧⲟⲥ", "Ⲭⲣⲓⲥⲧⲟⲥ", "ⲛⲓⲁ̀ⲅⲅⲉⲗⲟⲥ", "ⲓⲣⲏⲛⲏ", "ⲟⲩⲟϩ",

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

        static readonly string[] IpaTranscribe_CopticStandard_Samples = new[]
        {
            "}polic `m'u,on",
            "Taisoury `nnoub `nka;aroc etfai qa pi`arwmata@ etqen nenjij `n`Aarwn pi`ouyb eftale ou`c;oinoufi `e`pswi `ejen pima `n`ersw`ousi.",
        };

        [Fact]
        public void IpaTranscribe_CopticUnicode()
        {
            StringBuilder sb = new();

            foreach (var input in IpaTranscribe_CopticUnicode_Samples)
            {
                var result = CopticInterpreter.IpaTranscribe(input);

                Debug.WriteLine(result);
                sb.AppendLine($"{input},{result}");
            }

            string outPath = Path.Combine(AppContext.BaseDirectory, "TestResults", nameof(IpaTranscribe_CopticUnicode) + ".csv");
            Directory.CreateDirectory(Path.GetDirectoryName(outPath));
            File.WriteAllText(outPath, sb.ToString(), Encoding.Unicode);
        }

        [Fact]
        public void IpaTranscribe_CopticStandard()
        {
            foreach (var input in IpaTranscribe_CopticStandard_Samples)
            {
                string output = CopticInterpreter.IpaTranscribe(CopticFont.CsAvvaShenouda.Convert(input));
                Debug.WriteLine(output);
            }
        }
    }
}
