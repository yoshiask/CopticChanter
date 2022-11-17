using System.Collections.Generic;

namespace CoptLib.Writing
{
    internal static class IpaTables
    {
        public static IReadOnlyDictionary<Language, IReadOnlyDictionary<string, string>> IpaToLanguage = new Dictionary<Language, IReadOnlyDictionary<string, string>>
        {
            [Language.English] = IpaToEnglish(),
        };

        public static IReadOnlyDictionary<string, string> IpaToEnglish() => new Dictionary<string, string>
        {
            ["ä"] = "a",
            ["ɣ"] = "g",
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
            ["tʃ"] = "tch",
            ["ti"] = "tee",
            ["\u0300"] = "e",
            ["ɛ"] = ".e",
            ["ç"] = "ch",
            ["ŋ"] = "ng",
        };
    }
}
