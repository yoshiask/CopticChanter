using System.Collections.Generic;

namespace CoptLib.Writing;

partial class CopticFont
{
    public static List<CopticFont> Fonts = new()
    {
        CsAvvaShenouda, CsCopt, CsCopticManuscript, CsCoptoManuscript,
        CsKoptosManuscript, CsNewAthanasius, CsPishoi,
        CopticUnicode, Coptic1, Athanasius,
        GreekUnicode
    };

    public static CopticFont CsAvvaShenouda =>
        new()
        {
            Name = "CS Avva Shenouda",
            FontName = "CS Avva Shenouda",
            IsCopticStandard = true,
            IsJenkimBefore = true,
            Charmap = InitCopticStandard()
        };

    public static CopticFont CsCopt =>
        new()
        {
            Name = "CS Copt",
            FontName = "CS Copt",
            IsCopticStandard = true,
            IsJenkimBefore = true,
            Charmap = InitCopticStandard()
        };

    public static CopticFont CsCopticManuscript =>
        new()
        {
            Name = "CS Coptic Manuscript",
            FontName = "CS Coptic Manuscript",
            IsCopticStandard = true,
            IsJenkimBefore = true,
            Charmap = InitCopticStandard()
        };

    public static CopticFont CsCoptoManuscript =>
        new()
        {
            Name = "CS Copto Manuscript",
            FontName = "CS Copto Manuscript",
            IsCopticStandard = true,
            IsJenkimBefore = true,
            Charmap = InitCopticStandard()
        };

    public static CopticFont CsKoptosManuscript =>
        new()
        {
            Name = "CS Koptos Manuscript",
            FontName = "CS Koptos Manuscript",
            IsCopticStandard = true,
            IsJenkimBefore = true,
            Charmap = InitCopticStandard()
        };

    public static CopticFont CsNewAthanasius =>
        new()
        {
            Name = "CS New Athanasius",
            FontName = "CS New Athanasius",
            IsCopticStandard = true,
            IsJenkimBefore = true,
            Charmap = InitCopticStandard()
        };

    public static CopticFont CsPishoi =>
        new()
        {
            Name = "CS Pishoi",
            FontName = "CS Pishoi",
            IsCopticStandard = true,
            IsJenkimBefore = true,
            Charmap = InitCopticStandard()
        };

    public static CopticFont CopticUnicode =>
        new()
        {
            Name = "Coptic Unicode",
            FontName = "Segoe UI",
            IsCopticStandard = false,
            IsJenkimBefore = false,
            Charmap = InitCopticUnicode()
        };

    public static CopticFont Coptic1 =>
        new()
        {
            Name = "Coptic1",
            FontName = "Coptic1",
            IsCopticStandard = false,
            IsJenkimBefore = false,
            Charmap = new Dictionary<string, string>()
        };

    public static CopticFont Athanasius =>
        new()
        {
            Name = "Athanasius",
            FontName = "Athanasius Plain",
            IsCopticStandard = false,
            IsJenkimBefore = true,
            Charmap = InitAthanasuis()
        };

    public static CopticFont GreekUnicode =>
        new()
        {
            Name = "Greek Unicode",
            FontName = "Segoe UI",
            IsCopticStandard = false,
            IsJenkimBefore = false,
            Charmap = InitGreekUnicode()
        };

    
    private static Dictionary<string, string> InitCopticStandard()
    {
        Dictionary<string, string> alphabet = new()
        {
            // Key is Coptic Standard
            // Value is Coptic Standard

            #region Uppercase
            { "A", "A" },
            { "B", "B" },
            { "G", "G" },
            { "D", "D" },
            { "E", "E" },
            { "^", "^" },
            { "Z", "Z" },
            { "Y", "Y" },
            { ":", ":" },
            { "I", "I" },
            { "K", "K" },
            { "L", "L" },
            { "M", "M" },
            { "N", "N" },
            { "X", "X" },
            { "O", "O" },
            { "P", "P" },
            { "R", "R" },
            { "C", "C" },
            { "T", "T" },
            { "U", "U" },
            { "V", "V" },
            { "<", "<" },
            { "\"", "\"" },
            { "W", "W" },
            { "S", "S" },
            { "F", "F" },
            { "Q", "Q" },
            { "H", "H" },
            { "J", "J" },
            { "{", "{" },
            { "}", "}" },
            #endregion

            #region Lowercase
            { "a", "a" },
            { "b", "b" },
            { "g", "g" },
            { "d", "d" },
            { "e", "e" },
            { "6", "6" },
            { "z", "z" },
            { "y", "y" },
            { ";", ";" },
            { "i", "i" },
            { "k", "k" },
            { "l", "l" },
            { "m", "m" },
            { "n", "n" },
            { "x", "x" },
            { "o", "o" },
            { "p", "p" },
            { "r", "r" },
            { "c", "c" },
            { "t", "t" },
            { "u", "u" },
            { "v", "v" },
            { ",", "," },
            { "'", "'" },
            { "w", "w" },
            { "s", "s" },
            { "f", "f" },
            { "q", "q" },
            { "h", "h" },
            { "j", "j" },
            { "[", "[" },
            { "]", "]" },
            #endregion

            // u0300 is the combining grave accent
            // u200D is the zero-width joiner
            // NOTE: Some text renderers and fonts put the accent on the character before it
            { "`", "`" },
            // u0305 is the combining overline
            { "=", "=" },

            { "@", "@" },
            { "&", "&" },
            { "_", "_" },
        };

        return alphabet;
    }
    private static Dictionary<string, string> InitCopticUnicode()
    {
        Dictionary<string, string> alphabet = new()
        {
            // Key is Coptic Standard
            // Value is Coptic Unicode

            #region Uppercase
            { "A", "Ⲁ" },
            { "B", "Ⲃ" },
            { "G", "Ⲅ" },
            { "D", "Ⲇ" },
            { "E", "Ⲉ" },
            { "^", "Ⲋ" },
            { "Z", "Ⲍ" },
            { "Y", "Ⲏ" },
            { ":", "Ⲑ" },
            { "I", "Ⲓ" },
            { "K", "Ⲕ" },
            { "L", "Ⲗ" },
            { "M", "Ⲙ" },
            { "N", "Ⲛ" },
            { "X", "Ⲝ" },
            { "O", "Ⲟ" },
            { "P", "Ⲡ" },
            { "R", "Ⲣ" },
            { "C", "Ⲥ" },
            { "T", "Ⲧ" },
            { "U", "Ⲩ" },
            { "V", "Ⲫ" },
            { "<", "Ⲭ" },
            { "\"", "Ⲯ" },
            { "W", "Ⲱ" },
            { "S", "Ϣ" },
            { "F", "Ϥ" },
            { "Q", "Ϧ" },
            { "H", "Ϩ" },
            { "J", "Ϫ" },
            { "{", "Ϭ" },
            { "}", "Ϯ" },
            #endregion

            #region Lowercase
            { "a", "ⲁ" },
            { "b", "ⲃ" },
            { "g", "ⲅ" },
            { "d", "ⲇ" },
            { "e", "ⲉ" },
            { "6", "ⲋ" },
            { "z", "ⲍ" },
            { "y", "ⲏ" },
            { ";", "ⲑ" },
            { "i", "ⲓ" },
            { "k", "ⲕ" },
            { "l", "ⲗ" },
            { "m", "ⲙ" },
            { "n", "ⲛ" },
            { "x", "ⲝ" },
            { "o", "ⲟ" },
            { "p", "ⲡ" },
            { "r", "ⲣ" },
            { "c", "ⲥ" },
            { "t", "ⲧ" },
            { "u", "ⲩ" },
            { "v", "ⲫ" },
            { ",", "ⲭ" },
            { "'", "ⲯ" },
            { "w", "ⲱ" },
            { "s", "ϣ" },
            { "f", "ϥ" },
            { "q", "ϧ" },
            { "h", "ϩ" },
            { "j", "ϫ" },
            { "[", "ϭ" },
            { "]", "ϯ" },
            #endregion

            // Because we're using the "combining" versions of
            // these diacritics, we don't need to add a ZWJ.
            
            // u0300 is the combining grave accent
            { "`", "\u0300" },

            // u0305 is the combining overline
            { "=", "\u0305" },

            { "@", ":" },
            { "&", ";" },
            { "_", "=" },
            { "¡", "⳪" }
        };

        return alphabet;
    }
    private static Dictionary<string, string> InitAthanasuis()
    {
        Dictionary<string, string> alphabet = new()
        {
            // Key is Coptic Standard
            // Value is Athanasius

            #region Uppercase
            { "A", "A" },
            { "B", "B" },
            { "G", "G" },
            { "D", "D" },
            { "E", "E" },
            { "^", "," },
            { "Z", "Z" },
            { "Y", "H" },
            { ":", "Q" },
            { "I", "I" },
            { "K", "K" },
            { "L", "L" },
            { "M", "M" },
            { "N", "N" },
            { "X", "{" },
            { "O", "O" },
            { "P", "P" },
            { "R", "R" },
            { "C", "C" },
            { "T", "T" },
            { "U", "U" },
            { "V", "V" },
            { "<", "X" },
            { "\"", "Y" },
            { "W", "W" },
            { "S", "}" },
            { "F", "F" },
            { "Q", "\"" },
            { "H", "|" },
            { "J", "J" },
            { "{", "S" },
            { "}", ":" },
            #endregion

            #region Lowercase
            { "a", "a" },
            { "b", "b" },
            { "g", "g" },
            { "d", "d" },
            { "e", "e" },
            { "6", "6" },
            { "z", "z" },
            { "y", "h" },
            { ";", "q" },
            { "i", "i" },
            { "k", "k" },
            { "l", "l" },
            { "m", "m" },
            { "n", "n" },
            { "x", "[" },
            { "o", "o" },
            { "p", "p" },
            { "r", "r" },
            { "c", "c" },
            { "t", "t" },
            { "u", "u" },
            { "v", "v" },
            { ",", "ⲭ" },
            { "'", "y" },
            { "w", "w" },
            { "s", "]" },
            { "f", "f" },
            { "q", "\'" },
            { "h", "\\" },
            { "j", "j" },
            { "[", "s" },
            { "]", ";" },
            #endregion

            { "`", "`" },

            { "@", ">" },
            { "&", "^" },
            { "¡", "_" },

            { "=", "?" }
        };

        return alphabet;
    }
    private static Dictionary<string, string> InitGreekUnicode()
    {
        Dictionary<string, string> alphabet = new()
        {
            // Key is Coptic Standard
            // Value is Greek unicode

            #region Uppercase
            { "A", "Α" },
            { "B", "Β" },
            { "G", "Γ" },
            { "D", "Δ" },
            { "E", "Ε" },
            { "Z", "Ζ" },
            { "Y", "Η" },
            { ":", "Θ" },
            { "I", "Ι" },
            { "K", "Κ" },
            { "L", "Λ" },
            { "M", "Μ" },
            { "N", "Ν" },
            { "X", "Ξ" },
            { "O", "Ο" },
            { "P", "Π" },
            { "R", "Ρ" },
            { "C", "Σ" },
            { "T", "Τ" },
            { "U", "Υ" },
            { "V", "Φ" },
            { "<", "Χ" },
            { "\"", "Ψ" },
            { "W", "Ω" },

            { "S", "Ϣ" },
            { "F", "Ϥ" },
            { "Q", "Ϧ" },
            { "H", "Ϩ" },
            { "J", "Ϫ" },
            { "{", "Ϭ" },
            { "}", "Ϯ" },
            #endregion

            #region Lowercase
            { "a", "α" },
            { "b", "β" },
            { "g", "γ" },
            { "d", "δ" },
            { "e", "ε" },
            { "z", "ζ" },
            { "y", "η" },
            { ";", "θ" },
            { "i", "ι" },
            { "k", "κ" },
            { "l", "λ" },
            { "m", "μ" },
            { "n", "ν" },
            { "x", "ξ" },
            { "o", "ο" },
            { "p", "π" },
            { "r", "ρ" },
            { "c", "σ" },
            { "t", "τ" },
            { "u", "υ" },
            { "v", "φ" },
            { ",", "χ" },
            { "'", "ψ" },
            { "w", "ω" },
            { "s", "ς" },
            { "f", "ϥ" },
            { "q", "ϧ" },
            { "h", "ϩ" },
            { "j", "ϫ" },
            { "[", "ϭ" },
            { "]", "ϯ" },
            #endregion

            // u0300 is the combining grave accent
            // u200D is the zero-width joiner
            // NOTE: Some text renderers and fonts put the accent on the character before it
            { "`", "\u0300\u200D" },
            // u0305 is the combining overline
            { "=", "\u0305\u200D" },

            { "@", ":" },
            { "&", ";" },
            { "_", "=" },
            { "¡", "⳪" }
        };

        return alphabet;
    }
}
