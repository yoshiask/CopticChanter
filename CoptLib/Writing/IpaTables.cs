using System.Collections.Generic;

namespace CoptLib.Writing;

internal static class IpaTables
{
    public static readonly IReadOnlyDictionary<KnownLanguage, IReadOnlyDictionary<string, string>> IpaToLanguage =
        new Dictionary<KnownLanguage, IReadOnlyDictionary<string, string>>
    {
        [KnownLanguage.Arabic] = IpaToArabic(),
        [KnownLanguage.English] = IpaToEnglish(),
    };

    public static IReadOnlyDictionary<string, string> IpaToEnglish() => new Dictionary<string, string>
    {
        [Linguistics.PhoneticWord.DefaultSyllableSeparator] = "·",
        ["ä"] = "a\u0306",  // ă
        ["æ"] = "a",
        ["ɑ"] = "a\u0306",
        ["b\u032A"] = "b",
        ["g"] = "g",
        ["ɣ"] = "gʰ",
        ["ŋ"] = "n",
        ["ʤ"] = "j",
        ["d͡ʒ"] = "j",
        ["d\u032A"] = "d",
        ["ð"] = "dʰ",
        ["ɛ"] = "e",
        ["e\u031E"] = "e",
        ["z"] = "z",
        ["iː"] = "i",
        ["θ"] = "tʰ",
        ["i"] = "i\u0306",  // ĭ
        ["j"] = "y",
        ["ij"] = "i\u0306y",// ĭy
        ["e\u031Ej"] = "e\u0306y",// ĕy
        ["k"] = "k",
        ["l"] = "l",
        ["m"] = "m",
        ["n"] = "n",
        ["ks"] = "x",
        ["o\u031E"] = "o",
        ["p\u032A"] = "p",
        ["r"] = "r",
        ["ɾ"] = "r",
        ["s"] = "s",
        ["t\u032A"] = "t",
        ["ɪ"] = "i",
        ["ɸ"] = "f",
        ["ps"] = "ps",
        ["ʃ"] = "sʰ",
        ["χ"] = "kʰ",
        ["x"] = "kʰ",
        ["ç"] = "sʰ",
        ["h"] = "h",
        ["t\u0320ʃ"] = "cʰ",
        ["t\u032Ai"] = "ti",
    };

    public static IReadOnlyDictionary<string, string> IpaToArabic() => new Dictionary<string, string>
    {
        [Linguistics.PhoneticWord.DefaultSyllableSeparator] = "·",
        ["ä"] = "ا",
        ["b"] = "ب",
        ["ɣ"] = "غ",
        ["ð"] = "ذ",
        ["e\u031E"] = "ي",
        ["ɛ"] = "إ",
        ["z"] = "ز",
        ["iː"] = "ي",
        ["ɪ"] = "ي",
        ["θ"] = "ث",
        ["i"] = "ي",
        ["k"] = "ك",
        ["l"] = "ل",
        ["m"] = "م",
        ["n"] = "ن",
        ["ks"] = "كس",
        ["ŋ"] = "نج",
        ["o\u031E"] = "و",
        ["p"] = "پ",
        ["ɾ"] = "ر",
        ["s"] = "س",
        ["t"] = "ت",
        ["f"] = "ف",
        ["k"] = "ك",
        ["ps"] = "پس",
        ["ʃ"] = "ش",
        ["f"] = "ف",
        ["x"] = "خ",
        ["h"] = "ه",
        ["g"] = "ج",
        ["tʃ"] = "تش",
        ["ti"] = "تي",
        ["\u0300"] = "أ",    // jenkim
        ["w"] = "وو",
        ["u"] = "و",
        ["v"] = "ڤ",
        ["j"] = "ي",
        ["ij"] = "ي",
        ["ç"] = "ش",
        ["ʤ"] = "ج",
        ["d"] = "د",
    };
}