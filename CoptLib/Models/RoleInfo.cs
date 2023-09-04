using CoptLib.Models.Text;

namespace CoptLib.Models;

public class RoleInfo : Definition
{
    /// <summary>
    /// Creates a new <see cref="RoleInfo"/>.
    /// </summary>
    /// <param name="titleId">The common ID of the title or rank.</param>
    /// <param name="names">Translations of the title or rank's name.</param>
    public RoleInfo(string titleId, TranslationRunCollection names)
    {
        Key = titleId;
        Names = names;
    }

    /// <summary>
    /// The common ID of the title or rank.
    /// </summary>
    public string TitleId => Key!;

    /// <summary>
    /// Translations of the title or rank's name.
    /// </summary>
    public TranslationRunCollection Names { get; }

    // TODO: Move to a common definitions document

    public static readonly RoleInfo Pope = new("Pope", new()
    {
        new Run(null) { Text = "Pope", Language = new(Writing.KnownLanguage.English) },
        new Run(null) { Text = "Ⲡⲁⲡⲁ", Language = new(Writing.KnownLanguage.Coptic) },
        new Run(null) { Text = "بابا", Language = new(Writing.KnownLanguage.Arabic) },
        new Run(null) { Text = "Παπὰ", Language = new(Writing.KnownLanguage.Greek) },
        new Run(null) { Text = "Papa", Language = new(Writing.KnownLanguage.Italian) },
        new Run(null) { Text = "Papa", Language = new(Writing.KnownLanguage.Latin) },
        new Run(null) { Text = "Papa", Language = new(Writing.KnownLanguage.Spanish) },

        // TODO: Verify the following translations
        new Run(null) { Text = "ጳጳስ", Language = new(Writing.KnownLanguage.Amharic) },
        new Run(null) { Text = "պապը", Language = new(Writing.KnownLanguage.Armenian) },
        new Run(null) { Text = "Paus", Language = new(Writing.KnownLanguage.Dutch) },
        new Run(null) { Text = "Pape", Language = new(Writing.KnownLanguage.French) },
        new Run(null) { Text = "Papst", Language = new(Writing.KnownLanguage.German) },
        new Run(null) { Text = "אַפִּיפיוֹר", Language = new(Writing.KnownLanguage.Hebrew) },
    });

    public static readonly RoleInfo Priest = new("Priest", new()
    {
        new Run(null) { Text = "Priest", Language = new(Writing.KnownLanguage.English) },
        new Run(null) { Text = "Ⲡⲓⲟⲩⲏⲃ", Language = new(Writing.KnownLanguage.Coptic) },
        new Run(null) { Text = "الكاهنُ", Language = new(Writing.KnownLanguage.Arabic) },

        // TODO: Verify the following translations
        new Run(null) { Text = "ቄስ", Language = new(Writing.KnownLanguage.Amharic) },
        new Run(null) { Text = "Քահանա", Language = new(Writing.KnownLanguage.Armenian) },
        new Run(null) { Text = "Priester", Language = new(Writing.KnownLanguage.Dutch) },
        new Run(null) { Text = "Prêtre", Language = new(Writing.KnownLanguage.French) },
        new Run(null) { Text = "Sacerdote", Language = new(Writing.KnownLanguage.Italian) },
        new Run(null) { Text = "Priester", Language = new(Writing.KnownLanguage.German) },
        new Run(null) { Text = "Ιερεύς", Language = new(Writing.KnownLanguage.Greek) },
        new Run(null) { Text = "כּוֹמֶר", Language = new(Writing.KnownLanguage.Hebrew) },
        new Run(null) { Text = "Sacerdos", Language = new(Writing.KnownLanguage.Latin) },
        new Run(null) { Text = "Sacerdote", Language = new(Writing.KnownLanguage.Spanish) },
    });

    public static readonly RoleInfo Reader = new("Reader", new()
    {
        new Run(null) { Text = "Reader", Language = new(Writing.KnownLanguage.English) },
        new Run(null) { Text = "Ⲡⲓⲁ\u0300ⲛⲁⲅⲛⲱⲥⲧⲏⲥ", Language = new(Writing.KnownLanguage.Coptic) },
        new Run(null) { Text = "القارئُ", Language = new(Writing.KnownLanguage.Arabic) },
        new Run(null) { Text = "Ἀναγνώστης", Language = new(Writing.KnownLanguage.Greek) },

        // TODO: Verify the following translations
        new Run(null) { Text = "አንባቢ", Language = new(Writing.KnownLanguage.Amharic) },
        new Run(null) { Text = "Ընթերցող", Language = new(Writing.KnownLanguage.Armenian) },
        new Run(null) { Text = "Lezer", Language = new(Writing.KnownLanguage.Dutch) },
        new Run(null) { Text = "Lecteur", Language = new(Writing.KnownLanguage.French) },
        new Run(null) { Text = "Lettore", Language = new(Writing.KnownLanguage.Italian) },
        new Run(null) { Text = "Leser", Language = new(Writing.KnownLanguage.German) },
        new Run(null) { Text = "קוֹרֵא", Language = new(Writing.KnownLanguage.Hebrew) },
        new Run(null) { Text = "Lector", Language = new(Writing.KnownLanguage.Latin) },
        new Run(null) { Text = "Lector", Language = new(Writing.KnownLanguage.Spanish) },
    });

    public static readonly RoleInfo Deacon = new("Deacon", new()
    {
        new Run(null) { Text = "Deacon", Language = new(Writing.KnownLanguage.English) },
        new Run(null) { Text = "Ⲡⲓⲇⲓⲁⲕⲱⲛ", Language = new(Writing.KnownLanguage.Coptic) },
        new Run(null) { Text = "الشماس", Language = new(Writing.KnownLanguage.Arabic) },
        new Run(null) { Text = "Διάκονος", Language = new(Writing.KnownLanguage.Greek) },
        new Run(null) { Text = "ዲያቆን", Language = new(Writing.KnownLanguage.Amharic) },
        new Run(null) { Text = "Diaken", Language = new(Writing.KnownLanguage.Dutch) },
        new Run(null) { Text = "Diacre", Language = new(Writing.KnownLanguage.French) },
        new Run(null) { Text = "Diacono", Language = new(Writing.KnownLanguage.Italian) },
        new Run(null) { Text = "Diakon", Language = new(Writing.KnownLanguage.German) },
        new Run(null) { Text = "Diácono", Language = new(Writing.KnownLanguage.Spanish) },

        // TODO: Verify the following translations
        new Run(null) { Text = "սրկ", Language = new(Writing.KnownLanguage.Armenian) },
        new Run(null) { Text = "כּוֹמֶר זוּטָר", Language = new(Writing.KnownLanguage.Hebrew) },
        new Run(null) { Text = "Diacon", Language = new(Writing.KnownLanguage.Latin) },
    });

    public static readonly RoleInfo People = new("People", new()
    {
        new Run(null) { Text = "People", Language = new(Writing.KnownLanguage.English) },
        new Run(null) { Text = "Ⲡⲓⲗⲁⲟⲥ", Language = new(Writing.KnownLanguage.Coptic) },
        new Run(null) { Text = "الشعبُ", Language = new(Writing.KnownLanguage.Arabic) },

        // TODO: Verify the following translations
        new Run(null) { Text = "ሰዎች", Language = new(Writing.KnownLanguage.Amharic) },
        new Run(null) { Text = "Ժողովուրդ", Language = new(Writing.KnownLanguage.Armenian) },
        new Run(null) { Text = "Mensen", Language = new(Writing.KnownLanguage.Dutch) },
        new Run(null) { Text = "Gens", Language = new(Writing.KnownLanguage.French) },
        new Run(null) { Text = "Persone", Language = new(Writing.KnownLanguage.Italian) },
        new Run(null) { Text = "Menschen", Language = new(Writing.KnownLanguage.German) },
        new Run(null) { Text = "Λαός", Language = new(Writing.KnownLanguage.Greek) },
        new Run(null) { Text = "Personas", Language = new(Writing.KnownLanguage.Spanish) },
        new Run(null) { Text = "אֲנָשִׁים", Language = new(Writing.KnownLanguage.Hebrew) },
        new Run(null) { Text = "Homines", Language = new(Writing.KnownLanguage.Latin) },
    });

    public static readonly RoleInfo[] KnownRoles =
    {
        Pope, Priest, Reader, Deacon, People
    };
}