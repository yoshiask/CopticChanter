using CommunityToolkit.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CoptLib.Writing;

public class LanguageInfo : IEquatable<LanguageInfo>
{
    public const LanguageEquivalencyOptions DefaultLEO = LanguageEquivalencyOptions.StrictWithWild;
    
    /// <summary>
    /// Creates a new instance of <see cref="LanguageInfo"/> using the
    /// given RFC 5646 language tag.
    /// </summary>
    public LanguageInfo(string tag)
    {
        Tag = tag;

        string[] components = Tag.Split('-');
        
        Language = components[0];
        UpdateKnownFromSubtag(Language);

        if (components.Length >= 2)
        {
            Region = components[1];
            UpdateKnownFromSubtag($"{Language}-{Region}");

            if (components.Length >= 3)
            {
                Variant = components[2];
                UpdateKnownFromSubtag(Tag);
            }
        }
    }

    /// <summary>
    /// Creates a new instance of <see cref="LanguageInfo"/> using the known language name.
    /// </summary>
    public LanguageInfo(KnownLanguage known) : this(KnownLanguages.Keys.ElementAt((int)known))
    {
    }

    /// <summary>
    /// The RFC 5646 language tag.
    /// </summary>
    public string Tag { get; }

    public KnownLanguage Known { get; private set; }

    public string Language { get; }

    public string Region { get; }

    public string Variant { get; }

    public CultureInfo Culture { get; private set; }

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
            LanguageInfo primary = Parse(value[..idxSecondary]);
            primary.Secondary = Parse(value[(idxSecondary + 1)..]);
            return primary;
        }

        // Check if language is known
        if (Enum.TryParse(value, true, out KnownLanguage kLang))
        {
            return new(KnownLanguages.Keys.ElementAt((int)kLang));
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
        string str = Tag;
        if (Secondary is not null)
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

    public static bool IsNullOrDefault(LanguageInfo languageInfo) => languageInfo is null || languageInfo == Default;
    
    private static readonly IReadOnlyDictionary<string, KnownLanguage> KnownLanguages = new Dictionary<string, KnownLanguage>
    {
        ["und"]		    = KnownLanguage.Default,
        ["akk"]		    = KnownLanguage.Akkadian,
        ["am"]		    = KnownLanguage.Amharic,
        ["ar"]		    = KnownLanguage.Arabic,
        ["arc"]		    = KnownLanguage.Aramaic,
        ["hy"]		    = KnownLanguage.Armenian,
        ["cop"]		    = KnownLanguage.Coptic,
        ["nl"]		    = KnownLanguage.Dutch,
        ["egy"]		    = KnownLanguage.Egyptian,
        ["en"]		    = KnownLanguage.English,
        ["fr"]		    = KnownLanguage.French,
        ["ipa"]		    = KnownLanguage.IPA,    // Not an official ISO code
        ["it"]		    = KnownLanguage.Italian,
        ["de"]		    = KnownLanguage.German,
        ["el"]		    = KnownLanguage.Greek,
        ["he"]		    = KnownLanguage.Hebrew,
        ["la"]		    = KnownLanguage.Latin,
        ["es"]		    = KnownLanguage.Spanish,

        ["cop-boh"]     = KnownLanguage.CopticBohairic,
        ["cop-sah"]     = KnownLanguage.CopticSahidic,
        /// cop-akh     | Akhmimic
        /// cop-fay     | Fayyumic
        /// cop-her     | Hermopolitan
        /// cop-lyc     | Lycopolitan
        /// cop-oxy     | Oxyrhynchite
        /// cop-boh-EL  | Greco-Bohairic
    };

    public static readonly LanguageInfo Default = new(KnownLanguage.Default);

    private void UpdateKnownFromSubtag(string subtag)
    {
        if (KnownLanguages.ContainsKey(subtag))
            Known = KnownLanguages[subtag];

        var culture = OwlCore.Flow.Catch(() => CultureInfo.GetCultureInfo(subtag));
        if (culture is not null)
            Culture = culture;
    }
}
