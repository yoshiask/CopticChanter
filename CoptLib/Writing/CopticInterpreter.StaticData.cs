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

        public static readonly IReadOnlyDictionary<int, PhoneticEquivalent[]> KnownPronunciations = new Dictionary<int, PhoneticEquivalent[]>
        {
            ["ⲓⲏⲥⲟⲩⲥ".GetHashCode()] = PhoneticEquivalent.Parse("ⲓ,i;ⲏ,;ⲥ,s;ⲟ,;ⲩ,u;ⲥ,s"),
            ["ⲙⲁⲣⲓⲁ".GetHashCode()] = PhoneticEquivalent.Parse("ⲙ,m;ⲁ,ä;ⲣ,ɾ;ⲓ,i;ⲁ,ä"),
            ["ⲛⲓⲭⲉⲣⲟⲩⲃⲓⲙ".GetHashCode()] = PhoneticEquivalent.Parse("ⲛ,n;ⲓ,i;ⲭ,;ⲉ,e\u031E;ⲣ,ɾ;ⲟ,;ⲩ,u;ⲃ,b;ⲓ,i;ⲙ,m"),
            ["ⲛⲁⲣⲭⲏⲉⲣⲉⲩⲥ".GetHashCode()] = PhoneticEquivalent.Parse("\u0300,ɛ;ⲛ,n;\u0300,.;ⲁ,ä;ⲣ,ɾ;ⲭ,ʃ;ⲏ,i;\u0300,.;ⲉ,e\u031E;ⲣ,ɾ;ⲉ,e\u031E;ⲩ,v;ⲥ,s"),
            ["ⲥⲉⲇⲣⲁⲕ".GetHashCode()] = PhoneticEquivalent.Parse("ⲥ,s;ⲉ,e\u031E;ⲇ,d;ⲣ,ɾ;ⲁ,ä;ⲕ,k"),
            ["ⲁⲃⲇⲉⲛⲁⲅⲱ".GetHashCode()] = PhoneticEquivalent.Parse("ⲁ,ä;ⲃ,b;ⲇ,d;ⲉ,e\u031E;ⲛ,n;ⲁ,ä;ⲅ,g;ⲱ,o\u031E"),
            ["ⲓⲥⲭⲩⲣⲟⲛ".GetHashCode()] = PhoneticEquivalent.Parse("ⲓ,i;ⲥ,s;ⲭ,k;ⲩ,i;ⲣ,ɾ;ⲟ,o;ⲛ,n"),
            ["ⲇⲁⲩⲓⲇ".GetHashCode()] = PhoneticEquivalent.Parse("ⲇ,d;ⲁ,ä;ⲩ,v;ⲓ,i;ⲇ,d"),
            ["ⲁⲇⲁⲙ".GetHashCode()] = PhoneticEquivalent.Parse("ⲁ,ä;ⲇ,d;ⲁ,ä;ⲙ,m"),
            ["ⲁⲃⲃⲁ".GetHashCode()] = PhoneticEquivalent.Parse("ⲁ,ä;ⲃ,;ⲃ,v;ⲁ,ä"),
            ["ⲡⲁⲣⲭⲱⲛ".GetHashCode()] = PhoneticEquivalent.Parse("\u0300,ɛ;ⲡ,p;ⲁ,ä;ⲣ,ɾ;ⲭ,x;ⲱ,o\u031E;ⲛ,n"),
            ["ⲡⲓⲁⲭⲱⲣⲓⲧⲟⲥ".GetHashCode()] = PhoneticEquivalent.Parse("ⲡ,p;ⲓ,i;ⲁ,ä;ⲭ,k;ⲱ,o\u031E;ⲣ,ɾ;ⲓ,i;ⲧ,t;ⲟ,o;ⲥ,s"),
            ["ⲛⲓⲉⲩⲭⲏ".GetHashCode()] = PhoneticEquivalent.Parse("ⲛ,n;ⲓ,i;ⲉ,e;ⲩ,v;ⲭ,ʃ;ⲏ,i"),
            ["ⲛⲟⲩⲉⲩⲭⲏ".GetHashCode()] = PhoneticEquivalent.Parse("ⲛ,n;ⲟ,;ⲩ,u;\u0300,.;ⲉ,e;ⲩ,v;ⲭ,ʃ;ⲏ,i"),
            ["ⲅⲏ".GetHashCode()] = PhoneticEquivalent.Parse("ⲅ,g;ⲏ,e"),
            ["ⲭⲟⲓⲁⲕ".GetHashCode()] = PhoneticEquivalent.Parse("ⲭ,k;ⲟ,;ⲓ,i;ⲁ,ä;ⲕ,k"),
            ["ⲛⲛⲓⲡⲁⲭⲛⲏ".GetHashCode()] = PhoneticEquivalent.Parse("\u0300,ɛ;ⲛ,n;ⲛ,n;ⲓ,i;ⲡ,p;ⲁ,ä;ⲭ,ʃ;ⲛ,n;ⲏ,i"),
            ["ⲟⲛⲧⲟⲥ".GetHashCode()] = PhoneticEquivalent.Parse("ⲟ,o\u031E;ⲛ,n;ⲧ,d;ⲟ,o\u031E;ⲥ,s"),
        };
    }
}
