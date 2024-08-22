namespace CoptLib.Writing.Linguistics.XBar;

public enum PhrasalCategory : byte
{
    Unspecified,
    Adjective, Adverb, Prepositional, Clause, Conjunction, Determiner, Negation, Noun, Tense, Verb,

    Adpositional = Prepositional,
    Sentence = Clause,
}

public static class PhrasalCategories
{
    private static readonly DoubleDictionary<PhrasalCategory, string> _abbreviationMap = new()
    {
        [PhrasalCategory.Adjective] = "Adj",
        [PhrasalCategory.Adverb] = "Adv",
        [PhrasalCategory.Prepositional] = "P",
        [PhrasalCategory.Clause] = "C",
        [PhrasalCategory.Conjunction] = "Conj",
        [PhrasalCategory.Determiner] = "D",
        [PhrasalCategory.Negation] = "Neg",
        [PhrasalCategory.Noun] = "N",
        [PhrasalCategory.Tense] = "T",
        [PhrasalCategory.Verb] = "V",
        [PhrasalCategory.Unspecified] = "X",
    };

    public static string ToAbbreviation(this PhrasalCategory category) => _abbreviationMap[category];

    public static PhrasalCategory Parse(string str)
    {
        if (string.IsNullOrEmpty(str))
            return PhrasalCategory.Unspecified;

        if (_abbreviationMap.TryGetLeft(str, out var category))
            return category;

        // Check substrings if necessary
        if (str.Length > 4)
            return Parse(str[..4]);
        if (str.Length >= 2)
            return Parse(str[..^1]);

        return PhrasalCategory.Unspecified;
    }
}
