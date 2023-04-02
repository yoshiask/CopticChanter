using CoptLib.Writing.Linguistics.Analyzers;
using System;
using System.Collections.Generic;
using System.Linq;

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
}
