using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CoptLib.Writing;

public class LanguageInfo : IEquatable<LanguageInfo>
{
    /// <summary>
    /// Creates a new instance of <see cref="LanguageInfo"/> using the
    /// given RFC 5646 language tag.
    /// </summary>
    public LanguageInfo(string tag)
    {
        Tag = tag;

        string[] subtags = Tag.Split('-');
        Language = subtags[0];

        if (subtags.Length >= 2)
        {
            Region = subtags[1];

            if (subtags.Length >= 3)
                Variant = subtags[2];
        }

        try
        {
            Culture = CultureInfo.GetCultureInfo(Tag);
        }
        catch { }
    }

    /// <summary>
    /// Creates a new instance of <see cref="LanguageInfo"/> using the known language name.
    /// </summary>
    public LanguageInfo(KnownLanguage known) : this(KnownLanguages[known])
    {
        Known = known;
    }

    /// <summary>
    /// The RFC 5646 language tag.
    /// </summary>
    public string Tag { get; }

    public KnownLanguage Known { get; private set; }

    public string Language { get; }

    public string Region { get; }

    public string Variant { get; }

    public CultureInfo Culture { get; }

    /// <summary>
    /// A secondary content language, typically used for
    /// identifying transliterations or changes in script.
    /// </summary>
    public LanguageInfo Secondary { get; set; }

    /// <summary>
    /// Parses the given string as a <see cref="KnownLanguage"/> or
    /// RFC 5646-based language tag.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    public static LanguageInfo Parse(string value)
    {
        Guard.IsNotNull(value);

        int idxSecondary = value.IndexOf('/');
        if (idxSecondary > 0)
        {
            LanguageInfo primary = Parse(value.Substring(0, idxSecondary));
            primary.Secondary = new(value.Substring(idxSecondary + 1));
            return primary;
        }

        // Check if language is known
        if (Enum.TryParse(value, true, out KnownLanguage kLang)
            && KnownLanguages.TryGetValue(kLang, out var kLangTag))
        {
            return new(kLangTag)
            {
                Known = kLang
            };
        }

        return new(value);
    }

    /// <summary>
    /// Attempts to parse the given string as a <see cref="KnownLanguage"/> or
    /// RFC 5646-based language tag.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="info">The resulting <see cref="LanguageInfo"/> if parsing succeeded.</param>
    /// <returns></returns>
    public static bool TryParse(string value, out LanguageInfo info)
    {
        try
        {
            info = Parse(value);
            return true;
        }
        catch
        {
            info = null;
            return false;
        }
    }

    public override string ToString()
    {
        string str = Known != KnownLanguage.Default
            ? Known.ToString()
            : Tag;

        if (Secondary != null)
            str += $"/{Secondary}";

        return str;
    }

    /// <summary>
    /// Determines whether the specified language info is equivalent to the
    /// current language info using the given rules.
    /// </summary>
    /// <param name="other">The language info to compare against.</param>
    /// <param name="options">The rules to compare using.</param>
    /// <returns>
    /// <see langword="true"/> if the specified language is equivalent to the current info;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsEquivalentTo(LanguageInfo other, LanguageEquivalencyOptions options = LanguageEquivalencyOptions.Strict)
    {
        bool isEqual = true;
        bool useWild = options.HasFlag(LanguageEquivalencyOptions.TreatNullAsWild);

        if (other is null)
            return useWild;

        if (!useWild && options.HasFlag(LanguageEquivalencyOptions.Strict))
            return ToString().Equals(other?.ToString());

        if (options.HasFlag(LanguageEquivalencyOptions.Language) && isEqual)
            isEqual &= (useWild && (Language == null || other.Language == null))
                || this.Language == other.Language;

        if (options.HasFlag(LanguageEquivalencyOptions.Region) && isEqual)
            isEqual &= (useWild && (Region == null || other.Region == null))
                || this.Region == other.Region;

        if (options.HasFlag(LanguageEquivalencyOptions.Variant) && isEqual)
            isEqual &= (useWild && (Variant == null || other.Variant == null))
                || this.Variant == other.Variant;

        if (options.HasFlag(LanguageEquivalencyOptions.Secondary) && isEqual)
        {
            // Convert secondary options to primary
            var pri = (int)(options & LanguageEquivalencyOptions.Secondary) >> 3;

            // Replace all primary and secondary options
            options &= ~(LanguageEquivalencyOptions.Primary | LanguageEquivalencyOptions.Secondary);

            // Set secondary options as primary
            options |= (LanguageEquivalencyOptions)pri;

            // Compare secondary values
            isEqual &= IsEquivalentTo(this.Secondary, other.Secondary, options);
        }

        return isEqual;
    }

    public bool Equals(LanguageInfo other) => IsEquivalentTo(other);

    public static bool operator ==(LanguageInfo a, LanguageInfo b) => IsEquivalentTo(a, b);

    public static bool operator !=(LanguageInfo a, LanguageInfo b) => !(a == b);

    public override bool Equals(object obj) => Equals(obj as LanguageInfo);

    public override int GetHashCode() => ToString().GetHashCode();

    /// <inheritdoc cref="IsEquivalentTo(LanguageInfo, LanguageEquivalencyOptions)"/>
    public static bool IsEquivalentTo(LanguageInfo a, LanguageInfo b, LanguageEquivalencyOptions options = LanguageEquivalencyOptions.Strict)
    {
        if (a is null)
            return b is null || options.HasFlag(LanguageEquivalencyOptions.TreatNullAsWild);
        return a.IsEquivalentTo(b, options);
    }

    private static readonly IReadOnlyDictionary<KnownLanguage, string> KnownLanguages = new Dictionary<KnownLanguage, string>
    {
        [KnownLanguage.Default]	    = "und",
        [KnownLanguage.Akkadian]	= "akk",
        [KnownLanguage.Amharic]	    = "am",
        [KnownLanguage.Arabic]	    = "ar",
        [KnownLanguage.Aramaic]	    = "arc",
        [KnownLanguage.Armenian]	= "hy",
        [KnownLanguage.Coptic]	    = "cop",
        [KnownLanguage.Dutch]	    = "nl",
        [KnownLanguage.Egyptian]	= "egy",
        [KnownLanguage.English]	    = "en",
        [KnownLanguage.French]	    = "fr",
        [KnownLanguage.Italian]	    = "it",
        [KnownLanguage.German]	    = "de",
        [KnownLanguage.Greek]	    = "el",
        [KnownLanguage.Hebrew]	    = "he",
        [KnownLanguage.Latin]	    = "la",
        [KnownLanguage.Spanish]	    = "es",

        /// cop        | Coptic (generic)
        /// cop-EG-ALX | Bohairic (Alexandra)
        /// cop-EG-AST | Sahidic (Asyut)
        /// cop-EG-FYM | Fayyumic (Faiyum)
        /// cop-EG-SHG | Akhmimic (Sohag)
        /// cop-EG-MN  | Oxyrhynchite (Minya)
        /// cop-GR     | Greco-Bohairic
        [KnownLanguage.CopticBohairic]	    = "cop-EG-ALX",
        [KnownLanguage.CopticSahidic]	    = "cop-EG-AST",
    };

    public static readonly LanguageInfo Default = new(KnownLanguage.Default);
}
