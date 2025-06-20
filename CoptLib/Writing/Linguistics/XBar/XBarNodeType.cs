namespace CoptLib.Writing.Linguistics.XBar;

public enum XBarNodeType : byte
{
    LexicalItem, Phrase, Bar
}

public static class XBarNodeTypes
{
    private static readonly DoubleDictionary<XBarNodeType, string> AbbreviationMap = new()
    {
        [XBarNodeType.LexicalItem] = "°",
        [XBarNodeType.Phrase] = "P",
        [XBarNodeType.Bar] = "'",
    };

    public static string ToSuffix(this XBarNodeType type) => AbbreviationMap[type];

    public static XBarNodeType Parse(string str) => AbbreviationMap[str];

    public static (XBarNodeType, string) ParseSuffix(string str)
    {
        foreach (var (type, suffix) in AbbreviationMap)
        {
            if (!str.EndsWith(suffix))
                continue;
            
            return (type, str.Remove(str.Length - suffix.Length, suffix.Length));
        }

        return (XBarNodeType.LexicalItem, str);
    }
}
