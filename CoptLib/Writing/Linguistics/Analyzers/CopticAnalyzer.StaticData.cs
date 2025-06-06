﻿using System.Collections.Generic;
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
        ["ic"] = ("Ⲓⲏⲥⲟⲩⲥ", "Ⲓⲏ\u035Eⲥ"),
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
}