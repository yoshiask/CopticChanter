namespace CoptLib.Writing.Linguistics.XBar;

public enum XBarNodeType : byte
{
    PartOfSpeech, Phrase, Bar
}

public static class XBarNodeTypes
{
    private static readonly DoubleDictionary<XBarNodeType, string> _abbreviationMap = new()
    {
        [XBarNodeType.PartOfSpeech] = "",
        [XBarNodeType.Phrase] = "P",
        [XBarNodeType.Bar] = "'",
    };

    public static string ToSuffix(this XBarNodeType type) => _abbreviationMap[type];

    public static XBarNodeType Parse(string str) => _abbreviationMap[str];
}
