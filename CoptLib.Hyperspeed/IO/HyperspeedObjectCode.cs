namespace CoptLib.Hyperspeed.IO;

public enum HyperspeedObjectCode : ushort
{
    Null,
    Doc,
    Stanza,
    Section,
    Run,
    Comment,
    SimpleContent,
    Script,
    Variable,
    Translations,
    TranslationRuns,
    Set,
    Sequence,
    Author,
}

public enum HyperspeedSequenceNodeCode : byte
{
    Null,
    Constant,
    End,
    Scripted,
}