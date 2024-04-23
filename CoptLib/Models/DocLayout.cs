using CommunityToolkit.Diagnostics;
using CoptLib.Extensions;
using CoptLib.Writing;
using CoptLib.Writing.Linguistics;
using OwlCore.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CoptLib.Models;

public class DocLayout
{
    private DocLayoutOptions _options;
    private DocLayoutOptions? _lastOptions;
    private bool _isInvalidated = true;
    private List<List<IDefinition>>? _table;
    private readonly List<IDefinition> _docIntoList;

    /// <summary>
    /// Creates a new <see cref="DocLayout"/> for the specified document.
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="options"></param>
    public DocLayout(Doc doc, DocLayoutOptions? options = null)
    {
        Guard.IsNotNull(doc);

        Doc = doc;
        _options = options ?? new();
        _docIntoList = doc.IntoList<IDefinition>();
    }

    /// <summary>
    /// The document this layout is for.
    /// </summary>
    public Doc Doc { get; }

    /// <summary>
    /// Options used to configure how the layout is updated.
    /// </summary>
    public DocLayoutOptions Options
    {
        get => _options;
        set
        {
            _lastOptions = _options;
            _options = value;

            if (_options != _lastOptions)
                IsInvalidated = true;
        }
    }

    /// <summary>
    /// Whether the layout needs to be updated.
    /// </summary>
    [MemberNotNullWhen(false, nameof(_table))]
    public bool IsInvalidated
    {
        get => _isInvalidated;
        set
        {
            _isInvalidated = value;
            if (value)
            {
                Invalidated?.Invoke(this, null);
                _table = null;
            }
            else
            {
                // Ensure the table actually was generated
                Guard.IsNotNull(_table);
            }
        }
    }

    /// <summary>
    /// An event fired when the layout needs to be updated.
    /// </summary>
    public event EventHandler? Invalidated;

    /// <summary>
    /// A row-first flattened representation of the document contents.
    /// </summary>
    public List<List<IDefinition>> CreateTable()
    {
        if (!IsInvalidated)
            return _table;

        Guard.IsNotNull(Doc.Translations);

        var transliterations = Options.Transliterations.ToArray();

        var allLanguages = Doc.Translations.Children.Select(t => t.Language)
            .Concat(transliterations);
        var finalLanguages = (Options.IncludedLanguages ?? allLanguages)
            .Except(Options.ExcludedLanguages, LanguageInfoEqualityComparer.Strict)
            .ToArray();

        // Get the number of translations to display
        int translationCount = finalLanguages.Length + transliterations.Length;

        // Create rows for each stanza
        int rowCount = Doc.Translations.CountRows();
        _table = new(rowCount + 1)
        {
            // Add Doc to row so consumer can decide whether to show
            // the document name
            _docIntoList
        };

        // Pre-allocate rows
        for (int i = 0; i < rowCount; i++)
            _table.Add(new(translationCount));

        // Create final list of translations
        List<ContentPart> translations = new(translationCount);

        // Ignore translation if it's in the exclusion list but not the inclusion list
        bool LangFilter(LanguageInfo lang) => finalLanguages.Contains(lang, LanguageInfoEqualityComparer.Strict);
        foreach (var translation in Doc.Translations.Children)
            if (LangFilter(translation.Language))
                translations.Add(translation);

        // Perform any requested transliterations
        foreach (var transliterationLanguage in transliterations.Where(LangFilter))
        {
            var sourceLanguage = transliterationLanguage.GetPrimary();
            var targetLanguage = transliterationLanguage.Secondary
                ?? throw new ArgumentException("A target language must be specified for transliteration.");

            var source = Doc.Translations.GetByLanguage(sourceLanguage);
            Guard.IsNotNull(source);

            var transliteration = LinguisticLanguageService.Default.Transliterate(source, targetLanguage, sourceLanguage);

            // Try to place the transliteration right after its source
            int srcIndex = translations.IndexOf(source);
            int trsIndex = srcIndex >= 0 ? srcIndex + 1 : translations.Count;

            translations.Insert(trsIndex, (ContentPart)transliteration);
        }

        // Flatten each translation
        foreach (var translation in translations)
        {
            var flattenedTranslation = translation.Flatten().ToList();
            for (int i = 0; i < flattenedTranslation.Count; i++)
                _table[i + 1].Add(flattenedTranslation[i]);
        }

        IsInvalidated = false;

        return _table;
    }
}

public record DocLayoutOptions
{
    public DocLayoutOptions(IEnumerable<LanguageInfo>? includedLanguages = null, 
        IEnumerable<LanguageInfo>? excludedLanguages = null, IEnumerable<LanguageInfo>? transliterations = null)
    {
        IncludedLanguages = includedLanguages;
        ExcludedLanguages = excludedLanguages ?? Enumerable.Empty<LanguageInfo>();
        Transliterations = transliterations ?? Enumerable.Empty<LanguageInfo>();
    }

    /// <summary>
    /// A collection of languages to include in the layout.
    /// </summary>
    public IEnumerable<LanguageInfo>? IncludedLanguages { get; init; }

    /// <summary>
    /// A collection of languages to exclude from the layout,
    /// after the inclusion filter is applied.
    /// </summary>
    public IEnumerable<LanguageInfo> ExcludedLanguages { get; init; }

    /// <summary>
    /// A collection of languages to transliterate. Use <c>LanguageInfo.Primary</c>
    /// for the source language, and <c>Secondary</c> for the target.
    /// </summary>
    public IEnumerable<LanguageInfo> Transliterations { get; init; }
}
