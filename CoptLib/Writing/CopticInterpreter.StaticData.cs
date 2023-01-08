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
            ['ⲅ'] = "g",
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

        public static readonly IReadOnlyList<string> CopticPrefixes = new string[]
        {
            // Source: https://cld.bz/users/user-73469131/Coptic-Bohairic-Introductory-Course1

            /// Definite articles
            // Singular masculine
            "ⲡ̀", "ⲫ̀", "ⲡⲓ",
            // Singular feminine
            "ϯ", "ⲑ̀", "ⲧ̀",
            // Plural
            "ⲛⲓ", "ⲛⲉⲛ", "ⲛ̀",

            "ⲧⲓ",

            /// Indefinite articles
            // Singular (masculine and feminine)
            "ⲟⲩ",
            // Plural
            "ϩⲉⲛ", "ϩⲁⲛ",    // former is not Bohairic

            /// Possestive articles
            // Simple
            "ⲙ̀",
            // 1st singular
            // "ⲡⲁ", "ⲧⲁ", "ⲛⲁ",    // Ignored due to false positives
            // 2nd masculine singular
            "ⲡⲉⲕ", "ⲧⲉⲕ", "ⲛⲉⲕ",
            // 2nd feminine singular
            "ⲡⲉ", "ⲧⲉ", "ⲛⲉ",
            // 3rd masculine singular
            "ⲡⲉϥ", "ⲧⲉϥ", "ⲛⲉϥ",
            // 3rd feminine singular
            "ⲡⲉⲥ", "ⲧⲉⲥ", "ⲛⲉⲥ",
            // 1st plural
            "ⲡⲉⲛ", "ⲧⲉⲛ", //"ⲛⲉⲛ",
            // 2nd plural
            "ⲡⲉⲧⲉⲛ", "ⲧⲉⲧⲉⲛ", "ⲛⲉⲧⲉⲛ",
            // 3rd plural
            "ⲡⲟⲩ", "ⲧⲟⲩ", "ⲛⲟⲩ",

            /// Demonstrative adjectives
            "ⲡⲁⲓ", "ⲧⲁⲓ", "ⲛⲁⲓ",
        };

        public static readonly IReadOnlyDictionary<int, PhoneticEquivalent[]> KnownPronunciations = new Dictionary<int, PhoneticEquivalent[]>
        {
            // Yes, most of these are names and should be capitalized;
            // however, the CopyCasing step in the phonetic analysis will make
            // sure the end result is capitalized.

            ["ⲓⲏⲥⲟⲩⲥ".GetHashCode()] = PhoneticEquivalent.Parse("ⲓ,i;ⲏ,;ⲥ,s;ⲟ,;ⲩ,u;ⲥ,s"),
            ["ⲙⲁⲣⲓⲁ".GetHashCode()] = PhoneticEquivalent.Parse("ⲙ,m;ⲁ,ä;ⲣ,ɾ;ⲓ,i;ⲁ,ä"),
            ["ⲥⲉⲇⲣⲁⲕ".GetHashCode()] = PhoneticEquivalent.Parse("ⲥ,s;ⲉ,e\u031E;ⲇ,d;ⲣ,ɾ;ⲁ,ä;ⲕ,k"),
            ["ⲁⲃⲇⲉⲛⲁⲅⲱ".GetHashCode()] = PhoneticEquivalent.Parse("ⲁ,ä;ⲃ,b;ⲇ,d;ⲉ,e\u031E;ⲛ,n;ⲁ,ä;ⲅ,g;ⲱ,o\u031E"),
            ["ⲓⲥⲭⲩⲣⲟⲛ".GetHashCode()] = PhoneticEquivalent.Parse("ⲓ,i;ⲥ,s;ⲭ,k;ⲩ,i;ⲣ,ɾ;ⲟ,o;ⲛ,n"),
            ["ⲇⲁⲩⲓⲇ".GetHashCode()] = PhoneticEquivalent.Parse("ⲇ,d;ⲁ,ä;ⲩ,v;ⲓ,i;ⲇ,d"),
            ["ⲙⲓⲭⲁⲏⲗ".GetHashCode()] = PhoneticEquivalent.Parse("ⲙ,m;ⲓ,i;ⲭ,x;ⲁ,ä;ⲏ,iː;ⲗ,l"),
            ["ⲁⲇⲁⲙ".GetHashCode()] = PhoneticEquivalent.Parse("ⲁ,ä;ⲇ,d;ⲁ,ä;ⲙ,m"),
            ["ⲑⲉⲟ̀ⲇⲱⲣⲟⲥ".GetHashCode()] = PhoneticEquivalent.Parse("ⲑ,θ;ⲉ,e\u031E;\u0300,.;ⲟ,o;ⲇ,d;ⲱ,o\u031E;ⲣ,ɾ;ⲟ,o;ⲥ,s"),
            ["ⲁⲃⲃⲁ".GetHashCode()] = PhoneticEquivalent.Parse("ⲁ,ä;ⲃ,;ⲃ,v;ⲁ,ä"),
            ["ⲅⲏ".GetHashCode()] = PhoneticEquivalent.Parse("ⲅ,g;ⲏ,e"),
            ["ⲭⲟⲓⲁⲕ".GetHashCode()] = PhoneticEquivalent.Parse("ⲭ,k;ⲟ,;ⲓ,i;ⲁ,ä;ⲕ,k"),
            ["ⲃⲏⲑⲗⲉⲉⲙ".GetHashCode()] = PhoneticEquivalent.Parse("ⲃ,b;ⲏ,e\u031E;ⲑ,θ;ⲗ,l;ⲉ,e\u031E;ⲉ,e\u031E;ⲙ,m"),
            ["ⲭⲉⲣⲟⲩⲃⲓⲙ".GetHashCode()] = PhoneticEquivalent.Parse("ⲭ,ʃ;ⲉ,e\u031E;ⲣ,ɾ;ⲟ,;ⲩ,u;ⲃ,b;ⲓ,i;ⲙ,m"),
            ["ⲓⲟⲇⲉⲁ̀".GetHashCode()] = PhoneticEquivalent.Parse("ⲓ,j;ⲟ,o\u031E;ⲇ,d;ⲉ,e\u031E;\u0300,.;ⲁ,ä"),
            ["ⲓⲟⲩⲇⲁ".GetHashCode()] = PhoneticEquivalent.Parse("ⲓ,j;ⲟ,;ⲩ,u;ⲇ,d;ⲁ,ä"),
            ["ⲁⲗⲗⲏⲗⲟⲩⲓⲁ".GetHashCode()] = PhoneticEquivalent.Parse("ⲁ,ä;ⲗ,l;ⲗ,l;ⲏ,iː;ⲗ,l;ⲟ,;ⲩ,u;ⲓ,j;ⲁ,ä"),
        };

        public static readonly IReadOnlyDictionary<int, PhoneticEquivalent[]> KnownPronunciationsWithPrefix = new Dictionary<int, PhoneticEquivalent[]>
        {
            ["ⲟⲩⲟϩ".GetHashCode()] = PhoneticEquivalent.Parse("ⲟ,o\u031E;ⲩ,w;ⲟ,o\u031E;ϩ,h"),
        };
    }
}
