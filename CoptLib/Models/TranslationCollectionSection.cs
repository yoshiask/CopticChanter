﻿using CoptLib.Writing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace CoptLib.Models;

/// <summary>
/// Represents a collection of multiple <see cref="ContentPart"/>s that are
/// translations of the same content.
/// </summary>
[XmlRoot("Translations")]
public class TranslationCollectionSection : Section, ITranslationLookup<ContentPart>
{
    public TranslationCollectionSection(IDefinition? parent) : base(parent)
    {
    }

    /// <summary>
    /// Gets the first <see cref="ContentPart"/> that has the
    /// given key.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The content does not contain a translation with the given key.
    /// </exception>
    public ContentPart this[string key]
        => Children.First(t => t.Key is not null && t.Key.Equals(key, StringComparison.Ordinal));

    /// <summary>
    /// Gets the first <see cref="ContentPart"/> in the given <see cref="KnownLanguage"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// The content does not contain a translation for the given language.
    /// </exception>
    public ContentPart this[KnownLanguage lang] => GetByLanguage(lang);

    /// <summary>
    /// Gets the <see cref="ContentPart"/> at the given index.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="idx"/> is less than 0 or greater than or equal to the
    /// number of elements.
    /// </exception>
    public ContentPart this[int idx] => Children[idx];

    public override int CountRows()
    {
        // Unlike Sections, the content of a TranslationCollection
        // is meant to be shown in parallel (side-by-side) rather
        // than in series (one after the other).
        return Children.Count > 0
            ? Children.Max(cp => cp.CountRows())
            : 0;
    }

    public ContentPart GetByLanguage(KnownLanguage knownLanguage, Func<ContentPart, bool>? predicate = null)
    {
        var children = predicate is null
            ? Children : Children.Where(predicate).ToList();

        return children.FirstOrDefault(t => t.Language.Known == knownLanguage)
               ?? children.FirstOrDefault(t => t.Language.IsDefault())
               ?? throw new KeyNotFoundException($"No translation found for '{knownLanguage}'");
    }

    public ContentPart GetByLanguage(LanguageInfo language, Func<ContentPart, bool>? predicate = null, LanguageEquivalencyOptions options = LanguageInfo.DefaultLEO)
    {
        var children = predicate is null
            ? Children : Children.Where(predicate).ToList();

        return children.FirstOrDefault(t => t.Language.IsEquivalentTo(language, options))
           ?? children.FirstOrDefault(t => t.Language.IsDefault())
           ?? throw new KeyNotFoundException($"No translation found for '{language}'"); 
    }
}