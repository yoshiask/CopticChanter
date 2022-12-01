using System.Collections.Generic;

namespace CoptLib.Writing
{
    partial class CopticInterpreter
    {
        public static readonly char[] Separators = new[] { ' ', ',', ':', ';', '.' };
        public static readonly char[] Vowels = new[] { 'ⲁ', 'ⲉ', 'ⲓ', 'ⲏ', 'ⲟ', 'ⲱ' };
        public static readonly char[] CopticSpecificLetters = new[] { 'ϣ', 'ϥ', 'ϧ', 'ϫ', 'ϯ', 'ϭ' };
        public static readonly char[] GreekSpecificLetters = new[] { 'ⲅ', 'ⲇ', 'ⲍ', 'ⲝ', 'ⲯ' };
        public static readonly string[] GreekSpecificPrefixes = new[] { "ⲡⲣⲟ", "ⲡⲁⲣⲁ", "ⲁⲣⲭ", "ⲟⲙⲟ", "ⲕⲁⲧⲁ", "ⲥⲩⲛ" };
        public static readonly string[] GreekCommonSuffixes = new[] { "ⲁⲥ", "ⲟⲥ", "ⲏⲥ", "ⲁⲛ", "ⲟⲛ", "ⲏⲛ" };
        public static readonly IReadOnlyDictionary<char, string> SimpleIpaTranscriptions = new Dictionary<char, string>
        {
            ['ⲁ'] = "ä",
            ['ⲃ'] = "b",    // Always pronounced "v" in names
            ['ⲅ'] = "ɣ",
            ['ⲇ'] = "ð",    // Pronouned "d" in names
            ['ⲉ'] = "\u0065\u031E",
            ['ⲍ'] = "z",
            ['ⲏ'] = "iː",
            ['ⲑ'] = "θ",
            ['ⲓ'] = "i",
            ['ⲕ'] = "k",
            ['ⲗ'] = "l",
            ['ⲙ'] = "m",
            ['ⲛ'] = "n",
            ['ⲝ'] = "ks",   // Pronounced "eks" when at the start of a word
            ['ⲟ'] = "\u006F\u031E", // "ⲟⲩ" handled by VowelCombinations
            ['ⲡ'] = "p",
            ['ⲣ'] = "ɾ",
            ['ⲥ'] = "s",
            ['ⲧ'] = "t",
            ['ⲩ'] = "i",
            ['ⲫ'] = "f",
            ['ⲭ'] = "k",
            ['ⲯ'] = "ps",   // Pronounced "eps" when following a consonant
            ['ⲱ'] = "\u006F\u031E",
            ['ϣ'] = "ʃ",
            ['ϥ'] = "f",
            ['ϧ'] = "x",
            ['ϩ'] = "h",
            ['ϫ'] = "g",
            ['ϭ'] = "tʃ",   // Pronounced "etʃ" when following a consonant
            ['ϯ'] = "ti",
            ['\u0300'] = "ɛ"  // Jenkim splits syllable
        };
        public static readonly IReadOnlyDictionary<string, string> CopticAbbreviations = new Dictionary<string, string>
        {
            ["=o=c"] = "[oic",
            ["P=,=c"] = "Pi`,rictoc",
            ["=e=;=u"] = "e;ouab",
        };

    }
}
