using System.Collections.Generic;

namespace CoptLib.Writing.Linguistics.Analyzers
{
    partial class CopticAnalyzer
    {
        public static readonly char[] Vowels = new[] { 'ⲁ', 'ⲉ', 'ⲓ', 'ⲏ', 'ⲟ', 'ⲱ' };
        public static readonly char[] CopticSpecificLetters = new[] { 'ϣ', 'ϥ', 'ϧ', 'ϫ', 'ϯ', 'ϭ' };
        public static readonly char[] GreekSpecificLetters = new[] { 'ⲅ', 'ⲇ', 'ⲍ', 'ⲝ', 'ⲯ' };
        public static readonly string[] GreekSpecificPrefixes = new[] { "ⲡⲣⲟ", "ⲡⲁⲣⲁ", "ⲁⲣⲭ", "ⲟⲙⲟ", "ⲕⲁⲧⲁ", "ⲥⲩⲛ" };
        public static readonly string[] GreekCommonSuffixes = new[] { "ⲁⲥ", "ⲟⲥ", "ⲏⲥ", "ⲁⲛ", "ⲟⲛ", "ⲏⲛ" };

        public static readonly IReadOnlyDictionary<string, string> CopticAbbreviations = new Dictionary<string, string>
        {
            ["=o=c"] = "[oic",
            ["P=,=c"] = "Pi`,rictoc",
            ["=e=;=u"] = "e;ouab",
        };

        public static readonly IReadOnlyList<string> CopticPrefixes = new string[]
        {
            /// Unknown
            "ⲛ\u0300ⲛⲓ",

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
            // 2nd masculine singular
            "ⲡⲉⲕ", "ⲧⲉⲕ", "ⲛⲉⲕ",
            // 3rd masculine singular
            "ⲡⲉϥ", "ⲧⲉϥ", "ⲛⲉϥ",
            // 3rd feminine singular
            "ⲡⲉⲥ", "ⲧⲉⲥ", "ⲛⲉⲥ",
            // 1st plural
            "ⲡⲉⲛ", "ⲧⲉⲛ", "ⲛⲉⲛ",
            // 2nd plural
            "ⲡⲉⲧⲉⲛ", "ⲧⲉⲧⲉⲛ", "ⲛⲉⲧⲉⲛ",
            // 3rd plural
            "ⲡⲟⲩ", "ⲧⲟⲩ", "ⲛⲟⲩ",

            /// Demonstrative adjectives
            "ⲡⲁⲓ", "ⲧⲁⲓ", "ⲛⲁⲓ",

            // Possessive 1st singular
            "ⲡⲁ", "ⲧⲁ", "ⲛⲁ",
            // 2nd feminine singular
            "ⲡⲉ", "ⲧⲉ", "ⲛⲉ",
        };

        public static readonly IReadOnlyDictionary<int, PhoneticWord> KnownPronunciations = new Dictionary<int, PhoneticWord>
        {
            // Yes, most of these are names and should be capitalized;
            // however, the CopyCasing step in the phonetic analysis will make
            // sure the end result is capitalized.

            ["ⲓⲏⲥⲟⲩⲥ".GetHashCode()] = PhoneticWord.Parse("ⲓ,;ⲏ,iː", "ⲥ,s;ⲟ,;ⲩ,u;ⲥ,s"),
            ["ⲙⲁⲣⲓⲁ".GetHashCode()] = PhoneticWord.Parse("ⲙ,m;ⲁ,ɑ", "ⲣ,ɾ;ⲓ,i", "ⲁ,ɑ"),
            ["ⲥⲉⲇⲣⲁⲕ".GetHashCode()] = PhoneticWord.Parse("ⲥ,s;ⲉ,e\u031E;ⲇ,d", "ⲣ,ɾ;ⲁ,ɑ;ⲕ,k"),
            ["ⲁⲃⲇⲉⲛⲁⲅⲱ".GetHashCode()] = PhoneticWord.Parse("ⲁ,ɑ;ⲃ,b", "ⲇ,d;ⲉ,e\u031E", "ⲛ,n;ⲁ,ɑ", "ⲅ,g;ⲱ,o\u031E"),
            ["ⲓⲥⲭⲩⲣⲟⲛ".GetHashCode()] = PhoneticWord.Parse("ⲓ,i;ⲥ,s", "ⲭ,k;ⲩ,i", "ⲣ,ɾ;ⲟ,o;ⲛ,n"),
            ["ⲇⲁⲩⲓⲇ".GetHashCode()] = PhoneticWord.Parse("ⲇ,d;ⲁ,ɑ", "ⲩ,v;ⲓ,iː;ⲇ,d"),
            ["ⲙⲓⲭⲁⲏⲗ".GetHashCode()] = PhoneticWord.Parse("ⲙ,m;ⲓ,i", "ⲭ,x;ⲁ,ɑ", "ⲏ,iː;ⲗ,l"),
            ["ⲁⲇⲁⲙ".GetHashCode()] = PhoneticWord.Parse("ⲁ,ɑ", "ⲇ,d;ⲁ,ɑ;ⲙ,m"),
            ["ⲑⲉⲟ̀ⲇⲱⲣⲟⲥ".GetHashCode()] = PhoneticWord.Parse("ⲑ,θ;ⲉ,e\u031E", "ⲟ,o", "ⲇ,d;ⲱ,o\u031E", "ⲣ,ɾ;ⲟ,o;ⲥ,s"),
            ["ⲁⲃⲃⲁ".GetHashCode()] = PhoneticWord.Parse("ⲁ,ɑ;ⲃ,v", "ⲃ,v;ⲁ,ɑ"),
            ["ⲅⲏ".GetHashCode()] = PhoneticWord.Parse("ⲅ,g;ⲏ,e"),
            ["ⲭⲟⲓⲁⲕ".GetHashCode()] = PhoneticWord.Parse("ⲭ,k;ⲟ,;ⲓ,i", "ⲁ,ɑ;ⲕ,k"),
            ["ⲃⲏⲑⲗⲉⲉⲙ".GetHashCode()] = PhoneticWord.Parse("ⲃ,b;ⲏ,e\u031E;ⲑ,θ", "ⲗ,l;ⲉ,e\u031E", "ⲉ,e\u031E;ⲙ,m"),
            ["ⲭⲉⲣⲟⲩⲃⲓⲙ".GetHashCode()] = PhoneticWord.Parse("ⲭ,ʃ;ⲉ,e\u031E", "ⲣ,ɾ;ⲟ,;ⲩ,u", "ⲃ,b;ⲓ,i;ⲙ,m"),
            ["ⲓⲟⲇⲉⲁ̀".GetHashCode()] = PhoneticWord.Parse("ⲓ,j;ⲟ,o\u031E", "ⲇ,d;ⲉ,e\u031E", "ⲁ,ɑ"),
            ["ⲓⲟⲩⲇⲁ".GetHashCode()] = PhoneticWord.Parse("ⲓ,j;ⲟ,;ⲩ,u", "ⲇ,d;ⲁ,ɑ"),
            ["ⲁⲗⲗⲏⲗⲟⲩⲓⲁ".GetHashCode()] = PhoneticWord.Parse("ⲁ,ɑ;ⲗ,l", "ⲗ,l;ⲏ,iː", "ⲗ,l;ⲟ,;ⲩ,u", "ⲓ,j;ⲁ,ɑ"),
        };

        public static readonly IReadOnlyDictionary<int, PhoneticWord> KnownPronunciationsWithPrefix = new Dictionary<int, PhoneticWord>
        {
            ["ⲟⲩⲟϩ".GetHashCode()] = PhoneticWord.Parse("ⲟ,o\u031E", "ⲩ,w;ⲟ,o\u031E;ϩ,h"),
            ["ⲛⲉⲙ".GetHashCode()] = PhoneticWord.Parse("ⲛ,n;ⲉ,ɛ;ⲙ,m"),
        };
    }
}
