using CoptLib.Writing;

namespace CoptLib.Models;

/// <summary>
/// Represents any collection of <see cref="IMultilingual"/>s that can
/// be looked up by language.
/// </summary>
public interface ITranslationLookup
{
    /// <summary>
    /// Gets the first <typeparamref name="TMulti"/> in the collection of the given language.
    /// </summary>
    /// <typeparam name="TMulti">The type of multilingual content.</typeparam>
    /// <param name="knownLanguage">The language to search for.</param>
    TMulti GetByLanguage<TMulti>(KnownLanguage knownLanguage)
        where TMulti : IMultilingual;

    /// <summary>
    /// Gets the first <typeparamref name="TMulti"/> in the collection of the given language.
    /// </summary>
    /// <typeparam name="TMulti">The type of multilingual content.</typeparam>
    /// <param name="language">The language to check equivalency against.</param>
    /// <param name="options">The rules to compare using.</param>
    TMulti GetByLanguage<TMulti>(LanguageInfo language, LanguageEquivalencyOptions options = LanguageEquivalencyOptions.StrictWithWild)
        where TMulti : IMultilingual;
}
