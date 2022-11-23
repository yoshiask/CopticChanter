using System.Collections.Generic;

namespace CoptLib.Writing
{
    internal static class IpaTables
    {
        public static IReadOnlyDictionary<Language, IReadOnlyDictionary<string, string>> IpaToLanguage = new Dictionary<Language, IReadOnlyDictionary<string, string>>
        {
            [Language.Arabic] = IpaToArabic(),
            [Language.English] = IpaToEnglish(),
        };

        public static IReadOnlyDictionary<string, string> IpaToEnglish() => new Dictionary<string, string>
        {
            ["ä"] = "a",
            ["ɣ"] = "gh",
            ["ð"] = "dh",
            ["\u0065\u031E"] = "e",
            ["θ"] = "th",
            ["iː"] = "i",
            ["ɪ"] = "i",
            ["ks"] = "x",
            ["\u006F\u031E"] = "o",
            ["ɾ"] = "r",
            ["ʃ"] = "sh",
            ["x"] = "kh",
            ["dʒ"] = "j",
            ["tʃ"] = "ch",
            ["ti"] = "ti",
            ["\u0300"] = "e",
            ["ɛ"] = ".e",
            ["ç"] = "sh",
            ["ŋ"] = "ng",
            ["j"] = "y",
        };

        public static IReadOnlyDictionary<string, string> IpaToArabic() => new Dictionary<string, string>
        {
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
            ["ç"] = "ش",
        };
    }
}
