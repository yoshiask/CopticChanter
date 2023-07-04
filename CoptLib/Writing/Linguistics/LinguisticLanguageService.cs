using CoptLib.Writing.Linguistics.Analyzers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AngleSharp.Text;

namespace CoptLib.Writing.Linguistics;

/// <summary>
/// A service that allows for the registration and fetching of <see cref="LinguisticAnalyzer"/>s
/// for multiple languages.
/// </summary>
public class LinguisticLanguageService
{
    private readonly Dictionary<LanguageInfo, Func<LanguageInfo, LinguisticAnalyzer>> _factories;

    public static LinguisticLanguageService Default = new(new()
    {
        { new LanguageInfo(KnownLanguage.Coptic), lang => new CopticGrecoBohairicAnalyzer() },
        { new LanguageInfo(KnownLanguage.CopticBohairic), lang => new CopticOldBohairicAnalyzer() },
    });

    /// <summary>
    /// Creates an instance of <see cref="LinguisticLanguageService"/> with the given
    /// analyzers automatically registered.
    /// </summary>
    /// <param name="analyzerFactories">The analyzers to register.</param>
    public LinguisticLanguageService(Dictionary<LanguageInfo, Func<LanguageInfo, LinguisticAnalyzer>> analyzerFactories)
    {
        _factories = new(analyzerFactories);
    }

    /// <summary>
    /// Creates an empty instance of <see cref="LinguisticLanguageService"/>.
    /// </summary>
    public LinguisticLanguageService()
    {
        _factories = new();
    }

    /// <summary>
    /// Adds <paramref name="analyzerFactory"/> as an analyzer factory for <paramref name="language"/>.
    /// </summary>
    public void Register(LanguageInfo language, Func<LanguageInfo, LinguisticAnalyzer> analyzerFactory)
    {
        _factories.Add(language, analyzerFactory);
    }

    public LinguisticAnalyzer GetAnalyzerForLanguage(LanguageInfo languageInfo)
    {
        // Use wildcards if an exact match doesn't exist
        if (!_factories.TryGetValue(languageInfo, out var factory))
        {
            factory = _factories
                .First(kvp => kvp.Key.IsEquivalentTo(languageInfo, LanguageEquivalencyOptions.StrictWithWild))
                .Value;
        }

        return factory(languageInfo);
    }

    /// <summary>
    /// Attempts to identify what language a piece of text is written in.
    /// </summary>
    /// <param name="text">The text to analyze.</param>
    /// <param name="language">The identified language, if any.</param>
    /// <returns>Whether a language was identified.</returns>
    public static bool TryIdentifyLanguage(string text, out LanguageInfo language)
    {
        BucketCounter<KnownLanguage> languageCounts = new();
        
        foreach (var c in text)
        {
            if ((int)CharUnicodeInfo.GetUnicodeCategory(c) > 10 || c.IsWhiteSpaceCharacter())
                continue;

            ++languageCounts.Total;

            KnownLanguage charLang = c switch
            {
                >= 'A' and <= 'z' => KnownLanguage.English,
                >= 'Ϣ' and <= 'ϯ' or >= 'Ⲁ' and <= 'ⲱ' => KnownLanguage.Coptic,
                >= '\u0600' and <= '\u08FF' or >= '\uFB50' and <= '\uFEFC' => KnownLanguage.Arabic,
                >= 'Ά' and <= 'ώ' => KnownLanguage.Greek,
                _ => KnownLanguage.Default
            };

            ++languageCounts[charLang];
        }

        if (languageCounts.GetTotalPercent() < 0.10)
        {
            language = null;
            return false;
        }

        var knownLanguage = languageCounts.GetLargestBucket();
        if (knownLanguage == KnownLanguage.Greek)
        {
            // Make sure it really is Greek, and not just Coptic written
            // using the Greek Unicode section.
            var copticPercent = languageCounts.GetBucketPercent(KnownLanguage.Coptic);
            var greekPercent = languageCounts.GetBucketPercent(KnownLanguage.Greek);
            if (copticPercent >= 0.80 || (copticPercent > 0 && greekPercent >= 0.80))
                knownLanguage = KnownLanguage.Coptic;
        }

        language = new(knownLanguage);
        return true;
    }
}
