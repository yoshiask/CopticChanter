using CoptLib.Writing.Lexicon;
using CoptLib.Writing.Linguistics.XBar;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
// ReSharper disable InvalidXmlDocComment

namespace CoptLib.Writing.Linguistics.Analyzers;

partial class CopticAnalyzer
{
    public static readonly char[] Vowels = { 'ⲁ', 'ⲉ', 'ⲓ', 'ⲏ', 'ⲟ', 'ⲱ' };
    public static readonly char[] CopticSpecificLetters = { 'ϣ', 'ϥ', 'ϧ', 'ϫ', 'ϯ', 'ϭ' };
    public static readonly char[] GreekSpecificLetters = { 'ⲅ', 'ⲇ', 'ⲍ', 'ⲝ', 'ⲯ' };
    public static readonly string[] GreekSpecificPrefixes = { "ⲡⲣⲟ", "ⲡⲁⲣⲁ", "ⲁⲣⲭ", "ⲟⲙⲟ", "ⲕⲁⲧⲁ", "ⲥⲩⲛ" };
    public static readonly string[] GreekCommonSuffixes = { "ⲁⲥ", "ⲟⲥ", "ⲏⲥ", "ⲁⲛ", "ⲟⲛ", "ⲏⲛ" };

    protected static readonly Dictionary<string, (string ex, string sh)> CopticAbbreviations = new()
    {
        ["oc"] = ("ϭⲟⲓⲥ", "⳪"),
        ["poc"] = ("ⲡ\u0300ϭⲟⲓⲥ", "ⲡ\u0300ⲟ\u035Eⲥ"),
        ["xc"] = ("ⲭ\u0300ⲣⲓⲥⲧⲟⲥ", "ⲭ\u035Eⲥ"), // ⳩
        ["pxc"] = ("ⲡⲓⲭ\u0300ⲣⲓⲥⲧⲟⲥ", "ⲡⲓⲭ\u035Eⲥ"),
        ["fti"] = ("ⲫ\u0300ⲛⲟⲩϯ", "ⲫ\u035Eϯ"),
        ["ethu"] = ("ⲉⲑⲟⲩⲁⲃ", "ⲉ\u035Eⲑ\u035Eⲩ"),
        ["al"] = ("ⲁⲗⲗⲏⲗⲟⲩⲓⲁ", "ⲁ\u035Eⲗ"),
        ["pna"] = ("ⲡ\u0300ⲛⲉⲩⲙⲁ", "ⲡ\u035Eⲛ\u035Eⲁ"),
        ["mr"] = ("ⲙⲁⲣⲧⲩⲣⲟⲥ", "⳥"),
        ["ctc"] = ("ⲥⲧⲁⲩⲣⲟⲥ", "⳧"),
    };

    public static readonly IReadOnlyList<string> CopticPrefixes = new[]
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
        "ⲛⲓ", "ⲛⲉⲛ",

        "ⲧⲓ",

        /// Indefinite articles
        // Singular (masculine and feminine)
        "ⲟⲩ",
        // Plural
        "ϩⲉⲛ", "ϩⲁⲛ",    // former is not Bohairic

        /// Possestive articles
        // Simple
        "ⲙ̀", "ⲛ̀",
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
        ["ⲁⲃⲇⲉⲛⲁⲅⲱ".GetHashCode()] = PhoneticWord.Parse("ⲁ,ɑ;ⲃ,b", "ⲇ,d;ⲉ,e\u031E", "ⲛ,n;ⲁ,ɑ", "ⲅ,g;ⲱ,o"),
        ["ⲓⲥⲭⲩⲣⲟⲛ".GetHashCode()] = PhoneticWord.Parse("ⲓ,i;ⲥ,s", "ⲭ,k;ⲩ,i", "ⲣ,ɾ;ⲟ,o;ⲛ,n"),
        ["ⲇⲁⲩⲓⲇ".GetHashCode()] = PhoneticWord.Parse("ⲇ,d;ⲁ,ɑ", "ⲩ,v;ⲓ,iː;ⲇ,d"),
        ["ⲙⲓⲭⲁⲏⲗ".GetHashCode()] = PhoneticWord.Parse("ⲙ,m;ⲓ,i", "ⲭ,x;ⲁ,ɑ", "ⲏ,iː;ⲗ,l"),
        ["ⲁⲇⲁⲙ".GetHashCode()] = PhoneticWord.Parse("ⲁ,ɑ", "ⲇ,d;ⲁ,ɑ;ⲙ,m"),
        ["ⲑⲉⲟ̀ⲇⲱⲣⲟⲥ".GetHashCode()] = PhoneticWord.Parse("ⲑ,θ;ⲉ,e\u031E", "\u0300,;ⲟ,o", "ⲇ,d;ⲱ,o", "ⲣ,ɾ;ⲟ,o;ⲥ,s"),
        ["ⲁⲃⲃⲁ".GetHashCode()] = PhoneticWord.Parse("ⲁ,ɑ;ⲃ,v", "ⲃ,v;ⲁ,ɑ"),
        ["ⲅⲏ".GetHashCode()] = PhoneticWord.Parse("ⲅ,g;ⲏ,e"),
        ["ⲭⲟⲓⲁⲕ".GetHashCode()] = PhoneticWord.Parse("ⲭ,k;ⲟ,;ⲓ,i", "ⲁ,ɑ;ⲕ,k"),
        ["ⲃⲏⲑⲗⲉⲉⲙ".GetHashCode()] = PhoneticWord.Parse("ⲃ,b;ⲏ,e\u031E;ⲑ,θ", "ⲗ,l;ⲉ,e\u031E", "ⲉ,e\u031E;ⲙ,m"),
        ["ⲭⲉⲣⲟⲩⲃⲓⲙ".GetHashCode()] = PhoneticWord.Parse("ⲭ,ʃ;ⲉ,e\u031E", "ⲣ,ɾ;ⲟ,;ⲩ,u", "ⲃ,b;ⲓ,i;ⲙ,m"),
        ["ⲓⲟⲇⲉⲁ̀".GetHashCode()] = PhoneticWord.Parse("ⲓ,j;ⲟ,o", "ⲇ,d;ⲉ,e\u031E", "\u0300,;ⲁ,ɑ"),
        ["ⲓⲟⲩⲇⲁ".GetHashCode()] = PhoneticWord.Parse("ⲓ,j;ⲟ,;ⲩ,u", "ⲇ,d;ⲁ,ɑ"),
        ["ⲁⲗⲗⲏⲗⲟⲩⲓⲁ".GetHashCode()] = PhoneticWord.Parse("ⲁ,ɑ;ⲗ,l", "ⲗ,l;ⲏ,iː", "ⲗ,l;ⲟ,;ⲩ,u", "ⲓ,j;ⲁ,ɑ"),
    };

    public static readonly IReadOnlyDictionary<int, PhoneticWord> KnownPronunciationsWithPrefix = new Dictionary<int, PhoneticWord>
    {
        ["ⲟⲩⲟϩ".GetHashCode()] = PhoneticWord.Parse("ⲟ,o", "ⲩ,w;ⲟ,o;ϩ,h"),
        ["ⲛⲉⲙ".GetHashCode()] = PhoneticWord.Parse("ⲛ,n;ⲉ,ɛ;ⲙ,m"),
        ["ⲛⲁϩⲣⲉⲛ".GetHashCode()] = PhoneticWord.Parse("ⲛ,n;ⲁ,ɑ;ϩ,h", "ⲣ,ɾ;ⲉ,ɛ;ⲛ,n"),
    };

    public static readonly IReadOnlyDictionary<string, IDeterminerMeta> Determiners = new Dictionary<string, IDeterminerMeta>
    {
        // Definite
        ["ⲡ"] = new DeterminerArticleMeta(DeterminerStrength.Weak, true, new(Gender.Masculine, GrammaticalCount.Singular)),
        ["ⲫ"] = new DeterminerArticleMeta(DeterminerStrength.Weak, true, new(Gender.Masculine, GrammaticalCount.Singular)),
        ["ⲡⲓ"] = new DeterminerArticleMeta(DeterminerStrength.Strong, true, new(Gender.Masculine, GrammaticalCount.Singular)),
        ["ⲧ"] = new DeterminerArticleMeta(DeterminerStrength.Weak, true, new(Gender.Feminine, GrammaticalCount.Singular)),
        ["ⲑ"] = new DeterminerArticleMeta(DeterminerStrength.Weak, true, new(Gender.Feminine, GrammaticalCount.Singular)),
        ["ϯ"] = new DeterminerArticleMeta(DeterminerStrength.Strong, true, new(Gender.Feminine, GrammaticalCount.Singular)),
        ["ⲛⲓ"] = new DeterminerArticleMeta(DeterminerStrength.Strong, true, new(Number: GrammaticalCount.Plural)),

        // Indefinite
        ["ⲟⲩ"] = new DeterminerArticleMeta(default, false, new(Number: GrammaticalCount.Singular)),
        ["ϩⲁⲛ"] = new DeterminerArticleMeta(default, false, new(Number: GrammaticalCount.Plural)),

        // Possessive Strong
        ["ⲛ"] = new DeterminerPossessiveMeta(DeterminerStrength.Strong, NounMeta.Unspecified, NounMeta.Unspecified),
        ["ⲙ"] = new DeterminerPossessiveMeta(DeterminerStrength.Strong, NounMeta.Unspecified, NounMeta.Unspecified),

        // Possessive 1st Person
        ["ⲡⲁ"] = new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Singular, PointOfView.First), new(Gender.Masculine, GrammaticalCount.Singular)),
        ["ⲧⲁ"] = new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Singular, PointOfView.First), new(Gender.Feminine, GrammaticalCount.Singular)),
        ["ⲛⲁ"] = new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Singular, PointOfView.First), new(Number: GrammaticalCount.Plural)),
        ["ⲡⲉⲛ"] = new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.First), new(Gender.Masculine, GrammaticalCount.Singular)),
        ["ⲧⲉⲛ"] = new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.First), new(Gender.Feminine, GrammaticalCount.Singular)),
        ["ⲛⲉⲛ"] = new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.First), new(Number: GrammaticalCount.Plural)),

        // Possessive 2nd Person
        ["ⲡⲉⲕ"] = new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second), new(Gender.Masculine, GrammaticalCount.Singular)),
        ["ⲧⲉⲕ"] = new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second), new(Gender.Feminine, GrammaticalCount.Singular)),
        ["ⲛⲉⲕ"] = new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Second), new(Number: GrammaticalCount.Plural)),
        ["ⲡⲉ"] = new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second), new(Gender.Masculine, GrammaticalCount.Singular)),
        ["ⲧⲉ"] = new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second), new(Gender.Feminine, GrammaticalCount.Singular)),
        ["ⲛⲉ"] = new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Second), new(Number: GrammaticalCount.Plural)),
        ["ⲡⲉⲧⲉⲛ"] = new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Second), new(Gender.Masculine, GrammaticalCount.Singular)),
        ["ⲧⲉⲧⲉⲛ"] = new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Second), new(Gender.Feminine, GrammaticalCount.Singular)),
        ["ⲛⲉⲧⲉⲛ"] = new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Second), new(Number: GrammaticalCount.Plural)),

        // Possessive 3rd Person
        ["ⲡⲉϥ"] = new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third), new(Gender.Masculine, GrammaticalCount.Singular)),
        ["ⲧⲉϥ"] = new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third), new(Gender.Feminine, GrammaticalCount.Singular)),
        ["ⲛⲉϥ"] = new DeterminerPossessiveMeta(default, new(Gender.Masculine, GrammaticalCount.Singular, PointOfView.Third), new(Number: GrammaticalCount.Plural)),
        ["ⲡⲉⲥ"] = new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third), new(Gender.Masculine, GrammaticalCount.Singular)),
        ["ⲧⲉⲥ"] = new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third), new(Gender.Feminine, GrammaticalCount.Singular)),
        ["ⲛⲉⲥ"] = new DeterminerPossessiveMeta(default, new(Gender.Feminine, GrammaticalCount.Singular, PointOfView.Third), new(Number: GrammaticalCount.Plural)),
        ["ⲡⲟⲩ"] = new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Third), new(Gender.Masculine, GrammaticalCount.Singular)),
        ["ⲧⲟⲩ"] = new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Third), new(Gender.Feminine, GrammaticalCount.Singular)),
        ["ⲛⲟⲩ"] = new DeterminerPossessiveMeta(default, new(default, GrammaticalCount.Plural, PointOfView.Third), new(Number: GrammaticalCount.Plural)),

        // Demonstrative
        ["ⲡⲁⲓ"] = new DeterminerDemonstrativeMeta(DeterminerStrength.Near, new(Gender.Masculine, GrammaticalCount.Singular)),
        ["ⲧⲁⲓ"] = new DeterminerDemonstrativeMeta(DeterminerStrength.Near, new(Gender.Feminine, GrammaticalCount.Singular)),
        ["ⲛⲁⲓ"] = new DeterminerDemonstrativeMeta(DeterminerStrength.Near, new(Number: GrammaticalCount.Plural)),
        ["ⲉⲧⲉ ⲙⲙⲁⲩ"] = new DeterminerDemonstrativeMeta(DeterminerStrength.Far, NounMeta.Unspecified),

        // Quantifier
        ["ⲛⲓⲃⲉⲛ"] = new DeterminerQuantifyingMeta(new(Number: GrammaticalCount.All)),
    };

    public static readonly IReadOnlyDictionary<string, object> NounPrefixes = new Dictionary<string, object>
    {
        // Determiners
        ["ⲡ"] = Determiners["ⲡ"],
        ["ⲫ"] = Determiners["ⲫ"],
        ["ⲡⲓ"] = Determiners["ⲡⲓ"],
        ["ⲧ"] = Determiners["ⲧ"],
        ["ⲑ"] = Determiners["ⲑ"],
        ["ϯ"] = Determiners["ϯ"],
        ["ⲛⲓ"] = Determiners["ⲛⲓ"],
        ["ⲟⲩ"] = Determiners["ⲟⲩ"],
        ["ϩⲁⲛ"] = Determiners["ϩⲁⲛ"],
        ["ⲛ"] = Determiners["ⲛ"],
        ["ⲙ"] = Determiners["ⲙ"],
        ["ⲡⲁ"] = Determiners["ⲡⲁ"],
        ["ⲧⲁ"] = Determiners["ⲧⲁ"],
        ["ⲛⲁ"] = Determiners["ⲛⲁ"],
        ["ⲡⲉⲛ"] = Determiners["ⲡⲉⲛ"],
        ["ⲧⲉⲛ"] = Determiners["ⲧⲉⲛ"],
        ["ⲛⲉⲛ"] = Determiners["ⲛⲉⲛ"],
        ["ⲡⲉⲕ"] = Determiners["ⲡⲉⲕ"],
        ["ⲧⲉⲕ"] = Determiners["ⲧⲉⲕ"],
        ["ⲛⲉⲕ"] = Determiners["ⲛⲉⲕ"],
        ["ⲡⲉ"] = Determiners["ⲡⲉ"],
        ["ⲧⲉ"] = Determiners["ⲧⲉ"],
        ["ⲛⲉ"] = Determiners["ⲛⲉ"],
        ["ⲡⲉⲧⲉⲛ"] = Determiners["ⲡⲉⲧⲉⲛ"],
        ["ⲧⲉⲧⲉⲛ"] = Determiners["ⲧⲉⲧⲉⲛ"],
        ["ⲛⲉⲧⲉⲛ"] = Determiners["ⲛⲉⲧⲉⲛ"],
        ["ⲡⲉϥ"] = Determiners["ⲡⲉϥ"],
        ["ⲧⲉϥ"] = Determiners["ⲧⲉϥ"],
        ["ⲛⲉϥ"] = Determiners["ⲛⲉϥ"],
        ["ⲡⲉⲥ"] = Determiners["ⲡⲉⲥ"],
        ["ⲧⲉⲥ"] = Determiners["ⲧⲉⲥ"],
        ["ⲛⲉⲥ"] = Determiners["ⲛⲉⲥ"],
        ["ⲡⲟⲩ"] = Determiners["ⲡⲟⲩ"],
        ["ⲧⲟⲩ"] = Determiners["ⲧⲟⲩ"],
        ["ⲛⲟⲩ"] = Determiners["ⲛⲟⲩ"],
        ["ⲡⲁⲓ"] = Determiners["ⲡⲁⲓ"],
        ["ⲧⲁⲓ"] = Determiners["ⲧⲁⲓ"],
        ["ⲛⲁⲓ"] = Determiners["ⲛⲁⲓ"],

        // TODO: This lookup table should include all prefixes, including those that aren't strictly determiners
        //["ⲉ"] = null,
        //["ⲉⲟⲩ"] = null,
        //["ⲉⲩ"] = null,    // Contraction of above
    };

    public static readonly IReadOnlyList<(List<LazyRegex>, List<Func<Match, VerbMeta>>)> VerbPrefixes = new List<(List<LazyRegex>, List<Func<Match, VerbMeta>>)>
    {
        ([new LazyRegex(@"^((?<unt>ϣⲁ)|(?<yetn>ⲙⲡⲁ))?(?<pov>ϯ)(?<fut>ⲛⲁ)?(?<base>\w+)")],
            [m => new(new(RelativeTime.Present, m.Groups["fut"].Success ? RelativeTime.Future : default), MapVerbConjToPOV(m.Groups["pov"].Value))]),
        ([new LazyRegex(@"^((?<unt>ϣⲁ)|(?<yetn>ⲙⲡⲁ))?(?<pov>ϯ)(?<fut>ⲛⲁ)?(?<base>\w+)")],
            [m => new(new(RelativeTime.Present, m.Groups["fut"].Success ? RelativeTime.Future : default), MapVerbConjToPOV(m.Groups["pov"].Value))]),
    };

    private static NounMeta MapVerbConjToPOV(string prefix)
    {
        return prefix switch
        {
            // Part                    Gender               Sg./Pl.                          1st/2nd/3rd
            "ϯ" or "ⲓ"          => new(default,             GrammaticalCount.Singular,      PointOfView.First),
            
            "ⲕ" or "ⲭ"          => new(Gender.Masculine,    GrammaticalCount.Singular,      PointOfView.Second),
            "ϥ"                 => new(Gender.Masculine,    GrammaticalCount.Singular,      PointOfView.Third),

            "ⲉ" or "ⲧⲉ"          => new(Gender.Feminine,    GrammaticalCount.Singular,      PointOfView.Second),
            "ⲉ" or "ⲧⲉ"          => new(Gender.Feminine,    GrammaticalCount.Singular,      PointOfView.Third),
            
            "ⲧⲉⲧⲉⲛ"             => new(default,             GrammaticalCount.Plural,        PointOfView.First),
            "ⲟⲩ" or "ⲁⲩ"        => new(default,             GrammaticalCount.Plural,        PointOfView.Second),
            "ⲛ"                 => new(default,             GrammaticalCount.Plural,        PointOfView.Third),
            
            "ⲥ"                 => new(Gender.Feminine,     GrammaticalCount.Singular,      PointOfView.Third),
            _ => throw new ArgumentException(),
        };
    }
}