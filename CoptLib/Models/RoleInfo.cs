using CoptLib.Models.Text;
using CoptLib.Writing;

namespace CoptLib.Models;

public class RoleInfo : TranslationRunCollection
{
    /// <summary>
    /// Creates a new <see cref="RoleInfo"/>.
    /// </summary>
    /// <param name="titleId">The common ID of the title or rank.</param>
    public RoleInfo(string titleId) : base(titleId, null)
    {
    }

    /// <summary>
    /// The common ID of the title or rank.
    /// </summary>
    public string TitleId => Key!;

    private RoleInfo FluentAddText(string text, KnownLanguage language)
    {
        AddText(text, language);
        return this;
    }

    // TODO: Move to a common definitions document

    public static readonly RoleInfo Pope = new RoleInfo("Pope")
        .FluentAddText("Pope", KnownLanguage.English)
        .FluentAddText("Ⲡⲁⲡⲁ", KnownLanguage.Coptic)
        .FluentAddText("بابا", KnownLanguage.Arabic)
        .FluentAddText("Παπὰ", KnownLanguage.Greek)
        .FluentAddText("Papa", KnownLanguage.Italian)
        .FluentAddText("Papa", KnownLanguage.Latin)
        .FluentAddText("Papa", KnownLanguage.Spanish)
        .FluentAddText("ጳጳስ", KnownLanguage.Amharic)
        .FluentAddText("պապը", KnownLanguage.Armenian)
        .FluentAddText("Paus", KnownLanguage.Dutch)
        .FluentAddText("Pape", KnownLanguage.French)
        .FluentAddText("Papst", KnownLanguage.German)
        .FluentAddText("אַפִּיפיוֹר", KnownLanguage.Hebrew);


    public static readonly RoleInfo Priest = new RoleInfo("Priest")
        .FluentAddText("Priest", KnownLanguage.English)
        .FluentAddText("Ⲡⲓⲟⲩⲏⲃ", KnownLanguage.Coptic)
        .FluentAddText("الكاهنُ", KnownLanguage.Arabic)
        // TODO: Verify the following translations
        .FluentAddText("ቄስ", KnownLanguage.Amharic)
        .FluentAddText("Քահանա", KnownLanguage.Armenian)
        .FluentAddText("Priester", KnownLanguage.Dutch)
        .FluentAddText("Prêtre", KnownLanguage.French)
        .FluentAddText("Sacerdote", KnownLanguage.Italian)
        .FluentAddText("Priester", KnownLanguage.German)
        .FluentAddText("Ιερεύς", KnownLanguage.Greek)
        .FluentAddText("כּוֹמֶר", KnownLanguage.Hebrew)
        .FluentAddText("Sacerdos", KnownLanguage.Latin)
        .FluentAddText("Sacerdote", KnownLanguage.Spanish);

    public static readonly RoleInfo Reader = new RoleInfo("Reader")
        .FluentAddText("Reader", KnownLanguage.English)
        .FluentAddText("Ⲡⲓⲁ\u0300ⲛⲁⲅⲛⲱⲥⲧⲏⲥ", KnownLanguage.Coptic)
        .FluentAddText("القارئُ", KnownLanguage.Arabic)
        .FluentAddText("Ἀναγνώστης", KnownLanguage.Greek)
        // TODO: Verify the following translations
        .FluentAddText("አንባቢ", KnownLanguage.Amharic)
        .FluentAddText("Ընթերցող", KnownLanguage.Armenian)
        .FluentAddText("Lezer", KnownLanguage.Dutch)
        .FluentAddText("Lecteur", KnownLanguage.French)
        .FluentAddText("Lettore", KnownLanguage.Italian)
        .FluentAddText("Leser", KnownLanguage.German)
        .FluentAddText("קוֹרֵא", KnownLanguage.Hebrew)
        .FluentAddText("Lector", KnownLanguage.Latin)
        .FluentAddText("Lector", KnownLanguage.Spanish);

    public static readonly RoleInfo Deacon = new RoleInfo("Deacon")
        .FluentAddText("Deacon", KnownLanguage.English)
        .FluentAddText("Ⲡⲓⲇⲓⲁⲕⲱⲛ", KnownLanguage.Coptic)
        .FluentAddText("الشماس", KnownLanguage.Arabic)
        .FluentAddText("Διάκονος", KnownLanguage.Greek)
        .FluentAddText("ዲያቆን", KnownLanguage.Amharic)
        .FluentAddText("Diaken", KnownLanguage.Dutch)
        .FluentAddText("Diacre", KnownLanguage.French)
        .FluentAddText("Diacono", KnownLanguage.Italian)
        .FluentAddText("Diakon", KnownLanguage.German)
        .FluentAddText("Diácono", KnownLanguage.Spanish)
        // TODO: Verify the following translations
        .FluentAddText("սրկ", KnownLanguage.Armenian)
        .FluentAddText("כּוֹמֶר זוּטָר", KnownLanguage.Hebrew)
        .FluentAddText("Diacon", KnownLanguage.Latin);

    public static readonly RoleInfo People = new RoleInfo("People")
        .FluentAddText("People", KnownLanguage.English)
        .FluentAddText("Ⲡⲓⲗⲁⲟⲥ", KnownLanguage.Coptic)
        .FluentAddText("الشعبُ", KnownLanguage.Arabic)
        // TODO: Verify the following translations
        .FluentAddText("ሰዎች", KnownLanguage.Amharic)
        .FluentAddText("Ժողովուրդ", KnownLanguage.Armenian)
        .FluentAddText("Mensen", KnownLanguage.Dutch)
        .FluentAddText("Gens", KnownLanguage.French)
        .FluentAddText("Persone", KnownLanguage.Italian)
        .FluentAddText("Menschen", KnownLanguage.German)
        .FluentAddText("Λαός", KnownLanguage.Greek)
        .FluentAddText("Personas", KnownLanguage.Spanish)
        .FluentAddText("אֲנָשִׁים", KnownLanguage.Hebrew)
        .FluentAddText("Homines", KnownLanguage.Latin);

    public static readonly RoleInfo[] KnownRoles =
    {
        Pope, Priest, Reader, Deacon, People
    };
}