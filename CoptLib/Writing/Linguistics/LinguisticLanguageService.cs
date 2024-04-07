using AngleSharp.Text;
using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing.Linguistics.Analyzers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace CoptLib.Writing.Linguistics;

/// <summary>
/// A service that allows for the registration and fetching of <see cref="LinguisticAnalyzer"/>s
/// for multiple languages.
/// </summary>
public class LinguisticLanguageService
{
    private readonly Dictionary<LanguageInfo, Func<LanguageInfo, LinguisticAnalyzer>> _factories;

    public static readonly LinguisticLanguageService Default = new(new Dictionary<LanguageInfo, Func<LanguageInfo, LinguisticAnalyzer>>()
    {
        { new LanguageInfo(KnownLanguage.Coptic), lang => new CopticGrecoBohairicAnalyzer(lang) },
        { new LanguageInfo(KnownLanguage.CopticBohairic), lang => new CopticOldBohairicAnalyzer(lang) },
        { new LanguageInfo(KnownLanguage.Greek), lang => new GreekAnalyzer(lang) },
    });

    /// <summary>
    /// Creates an instance of <see cref="LinguisticLanguageService"/> with the given
    /// analyzers automatically registered.
    /// </summary>
    /// <param name="analyzerFactories">The analyzers to register.</param>
    public LinguisticLanguageService(IDictionary<LanguageInfo, Func<LanguageInfo, LinguisticAnalyzer>> analyzerFactories)
    {
        _factories = new(analyzerFactories);
    }

    /// <summary>
    /// Creates an empty instance of <see cref="LinguisticLanguageService"/>.
    /// </summary>
    public LinguisticLanguageService() : this(new Dictionary<LanguageInfo, Func<LanguageInfo, LinguisticAnalyzer>>())
    {
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
    /// Transliterates an <see cref="IDefinition"/> to the specified script.
    /// </summary>
    /// <param name="source">The content to transliterate.</param>
    /// <param name="targetLanguage">The script to write the transliteration using.</param>
    /// <param name="sourceLanguage">
    /// The language of the source content.
    /// <para>Pass <see langword="null"/> to attempt to infer the language.</para>
    /// </param>
    /// <returns>A transliteration of the <paramref name="source"/>.</returns>
    public IDefinition Transliterate(IDefinition source, LanguageInfo targetLanguage, LanguageInfo? sourceLanguage = null, SyllableSeparatorSet? syllableSeparator = null)
    {
        var analyzer = GetAnalyzerForLanguage(sourceLanguage ?? source.GetLanguage());
        return source.Select(Transliterate);

        void Transliterate(IDefinition def)
        {
            if (def is Run run)
                run.Text = analyzer.Transliterate(run.Text, targetLanguage.Known, syllableSeparator);

            if (def is IMultilingual multi)
            {
                // Ensure that the language and font are set.
                // Set secondary language to indicate transliteration.
                if (!multi.Language.IsDefault())
                    multi.Language.Secondary = targetLanguage;
                else
                    multi.Language = targetLanguage;

                multi.Font = null;
            }

            def.IsExplicitlyDefined = false;
        }
    }

    /// <summary>
    /// Attempts to identify what language a piece of text is written in.
    /// </summary>
    /// <param name="text">The text to analyze.</param>
    /// <param name="language">The identified language, if any.</param>
    /// <returns>Whether a language was identified.</returns>
    public static bool TryIdentifyLanguage(string text, [NotNullWhen(true)] out LanguageInfo? language)
    {
        BucketCounter<KnownLanguage> languageCounts = new();

        var span = text.AsSpan();
        for (int c = 0; c < span.Length; c++)
        {
            char ch = span[c];
            if ((int)CharUnicodeInfo.GetUnicodeCategory(ch) > 10 || ch.IsWhiteSpaceCharacter())
                continue;

            ++languageCounts.Total;

            KnownLanguage charLang = ch switch
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
            for (int c = 0; c < span.Length; c++)
            {
                char ch = span[c];
                if (ch >= 'Ϣ' && ch <= 'ϯ' && (ch != 'Ϩ' || ch != 'ϩ'))
                {
                    knownLanguage = KnownLanguage.Coptic;
                    break;
                }
            }
        }

        language = new(knownLanguage);
        return true;
    }
}
