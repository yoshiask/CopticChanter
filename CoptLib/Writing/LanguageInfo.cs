using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace CoptLib.Writing;

public class LanguageInfo
{
    /// <summary>
    /// Creates a new instance of <see cref="LanguageInfo"/> using the
    /// given RFC 5646 language tag.
    /// </summary>
    public LanguageInfo(string tag)
    {
        Tag = tag;

        int idxSecondary = tag.IndexOf('/');
        if (idxSecondary > 0)
        {
            Tag = tag.Substring(0, idxSecondary);
            Secondary = new(tag.Substring(idxSecondary + 1));
        }

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
    /// Parses the given string as a <see cref="KnownLanguage"/> or RFC 5646
    /// language tag.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    public static LanguageInfo Parse(string value)
    {
        Guard.IsNotNull(value);

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

    public override string ToString()
    {
        if (Known != KnownLanguage.Default)
            return Known.ToString();

        string tag = Tag;
        if (Secondary != null)
            tag += $"/{Secondary}";

        return tag;
    }

    /// <summary>
    /// Determines whether the specified tag is strictly equal to the current tag.
    /// </summary>
    /// <param name="obj">The <see cref="LanguageInfo"/> to compare with the current object.</param>
    /// <returns>
    /// <see langword="true"/> if the specified tag is equal to the current object;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(object obj)
    {
        if (obj is null)
            return false;

        return this.ToString().Equals(obj.ToString());
    }

    /// <summary>
    /// Determines whether the specified language info is equivalent to the
    /// current language info using the given rules.
    /// </summary>
    /// <param name="otherInfo">The language info to compare against.</param>
    /// <param name="options">The rules to compare using.</param>
    /// <returns>
    /// <see langword="true"/> if the specified language is equivalent to the current info;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsEquivalentTo(LanguageInfo otherInfo, LanguageEquivalencyOptions options = LanguageEquivalencyOptions.Strict)
    {
        bool isEqual = true;
        bool useWild = options.HasFlag(LanguageEquivalencyOptions.TreatNullAsWild);

        if (useWild && otherInfo == null)
            return true;

        if (!useWild && options.HasFlag(LanguageEquivalencyOptions.Strict))
            return Equals(otherInfo);

        if (options.HasFlag(LanguageEquivalencyOptions.Language) && isEqual)
            isEqual &= (useWild && (Language == null || otherInfo.Language == null))
                || this.Language.Equals(otherInfo.Language);

        if (options.HasFlag(LanguageEquivalencyOptions.Region) && isEqual)
            isEqual &= (useWild && (Region == null || otherInfo.Region == null))
                || this.Region.Equals(otherInfo.Region);

        if (options.HasFlag(LanguageEquivalencyOptions.Variant) && isEqual)
            isEqual &= (useWild && (Variant == null || otherInfo.Variant == null))
                || this.Variant.Equals(otherInfo.Variant);

        if (options.HasFlag(LanguageEquivalencyOptions.Secondary) && isEqual)
        {
            // Convert secondary options to primary
            var pri = (int)(options & LanguageEquivalencyOptions.Secondary) >> 3;

            // Replace all primary and secondary options
            options &= ~(LanguageEquivalencyOptions.Primary | LanguageEquivalencyOptions.Secondary);

            // Set secondary options as primary
            options |= (LanguageEquivalencyOptions)pri;

            // Compare secondary values
            isEqual &= Secondary == null ? useWild : Secondary.IsEquivalentTo(otherInfo.Secondary, options);
        }

        return isEqual;
    }

    private static readonly IReadOnlyDictionary<KnownLanguage, string> KnownLanguages = new Dictionary<KnownLanguage, string>
    {
        [KnownLanguage.Default]	    = "und",
        [KnownLanguage.Akkadian]	= "akk",
        [KnownLanguage.Amharic]	    = "am",
        [KnownLanguage.Arabic]	    = "ar",
        [KnownLanguage.Aramaic]	    = "arc",
        [KnownLanguage.Armenian]	= "hy",
        [KnownLanguage.Coptic]	    = "cop-GR",
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
    };

    public static readonly LanguageInfo Default = new(KnownLanguage.Default);
}
