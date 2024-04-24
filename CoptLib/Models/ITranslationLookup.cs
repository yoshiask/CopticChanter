using System;
using System.Diagnostics.CodeAnalysis;
using CoptLib.Writing;

namespace CoptLib.Models;

/// <summary>
/// Represents any collection of <see cref="IMultilingual"/>s that can
/// be looked up by language.
/// </summary>
public interface ITranslationLookup<out T> where T : IMultilingual
{
    /// <summary>
    /// Gets the first <typeparamref name="T"/> in the collection of the given language.
    /// </summary>
    /// <param name="knownLanguage">The language to search for.</param>
    /// <param name="predicate">An additional results filter.</param>
    T GetByLanguage(KnownLanguage knownLanguage, Func<T, bool>? predicate = null);

    /// <summary>
    /// Gets the first <typeparamref name="T"/> in the collection of the given language.
    /// </summary>
    /// <param name="language">The language to check equivalency against.</param>
    /// <param name="predicate">An additional results filter.</param>
    /// <param name="options">The rules to compare using.</param>
    T GetByLanguage(LanguageInfo language, Func<T, bool>? predicate = null, LanguageEquivalencyOptions options = LanguageInfo.DefaultLEO);
}

/// <inheritdoc />
public interface ITranslationLookup : ITranslationLookup<IMultilingual>
{
}

public static class TranslationLookupExtensions
{
    public static bool TryGetByLanguage<T>(this ITranslationLookup<T> lookup, LanguageInfo language,
        [NotNullWhen(true)] out T? item,
        Func<T, bool>? predicate = null, LanguageEquivalencyOptions options = LanguageInfo.DefaultLEO)
        where T : IMultilingual
    {
        try
        {
            item = lookup.GetByLanguage(language, predicate, options);
            return true;
        }
        catch
        {
            item = default;
            return false;
        }
    }
}