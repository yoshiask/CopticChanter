using CommunityToolkit.Diagnostics;
using CoptLib.Extensions;
using CoptLib.Writing;
using OwlCore.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CoptLib.Models;

public class DocLayout
{
    private DocLayoutOptions _options;
    private bool _isInvalidated = true;
    private List<List<object>>? _table;
    private readonly List<object> _docIntoList;

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
        _docIntoList = doc.IntoList<object>();
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
            if (_options != value)
                _isInvalidated = true;
            _options = value;
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
                Invalidated?.Invoke(this, null);
            else
                _table = null;
        }
    }

    /// <summary>
    /// An event fired when the layout needs to be updated.
    /// </summary>
    public event EventHandler? Invalidated;

    /// <summary>
    /// A row-first flattened representation of the document contents.
    /// </summary>
    public List<List<object>> CreateTable()
    {
        if (!IsInvalidated)
            return _table;

        Guard.IsNotNull(Doc.Translations);

        var allTranslations = Doc.Translations.Children.Select(t => t.Language);
        LanguageInfo[] finalTranslations = (Options.IncludedLanguages ?? allTranslations)
            .Except(Options.ExcludedLanguages, LanguageInfoEqualityComparer.Strict)
            .ToArray();

        // Get the number of translations to display
        int translationCount = finalTranslations.Length;

        // Create rows for each stanza
        int numRows = Doc.Translations.CountRows();
        _table = new(numRows + 1)
        {
            // Add Doc to row so consumer can decide whether to show
            // the document name
            _docIntoList
        };

        for (int i = 0; i < numRows; i++)
            _table.Add(new(translationCount));

        for (int t = 0; t < Doc.Translations.Children.Count; t++)
        {
            var translation = Doc.Translations.Children[t];

            // Ignore translation if it's in the exclusion list but not the inclusion list
            if (!finalTranslations.Contains(translation.Language, LanguageInfoEqualityComparer.StrictWithWild))
                continue;

            var flattenedTranslation = translation
                .Flatten(p => p is IContentCollectionContainer coll ? coll.Children : null)
                // The row count excludes sections without headers,
                // so we have to do the same here.
                .Where(p => p is Section section ? section.Title != null : p != null)
                .ToList<object>();

            foreach (var (elem, i) in flattenedTranslation.WithIndex())
                _table[i + 1].Add(elem);
        }

        return _table;
    }
}

public class DocLayoutOptions
{
    /// <summary>
    /// A collection of languages to include in the layout.
    /// </summary>
    public IEnumerable<LanguageInfo>? IncludedLanguages { get; set; }

    /// <summary>
    /// A collection of languages to exclude from the layout,
    /// after the inclusion filter is applied.
    /// </summary>
    public IEnumerable<LanguageInfo> ExcludedLanguages { get; set; } = Array.Empty<LanguageInfo>();
}
