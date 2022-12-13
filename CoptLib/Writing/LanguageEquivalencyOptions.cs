using System;

namespace CoptLib.Writing;

[Flags]
public enum LanguageEquivalencyOptions
{
    /// <summary>
    /// The tags much match exactly.
    /// </summary>
    Strict = ~TreatNullAsWild,

    /// <summary>
    /// The language must match.
    /// </summary>
    Language = 1 << 0,

    /// <summary>
    /// The region must match.
    /// </summary>
    Region = 1 << 1,

    /// <summary>
    /// The variant must match.
    /// </summary>
    Variant = 1 << 2,

    /// <summary>
    /// The secondary language must match.
    /// </summary>
    SecondaryLanguage = 1 << 3,

    /// <summary>
    /// The secondary region must match.
    /// </summary>
    SecondaryRegion = 1 << 4,

    /// <summary>
    /// The secondary variant must match.
    /// </summary>
    SecondaryVariant = 1 << 5,

    /// <summary>
    /// If either of the values being compared are null,
    /// the two are considered equal.
    /// </summary>
    TreatNullAsWild = 1 << 6,

    /// <summary>
    /// The language, region, and variant must match.
    /// Does not check secondary info.
    /// </summary>
    Primary = Language | Region | Variant,

    /// <summary>
    /// The language and region must match.
    /// Does not check variant or secondary info.
    /// </summary>
    LanguageRegion = Language | Region,

    /// <summary>
    /// The secondary language, region, and variant must match.
    /// Does not check primary info.
    /// </summary>
    Secondary = SecondaryLanguage | SecondaryRegion | SecondaryVariant,

    /// <summary>
    /// The secondary language and region must match.
    /// Does not check primary language or secondary variant.
    /// </summary>
    SecondaryLanguageRegion = SecondaryLanguage | SecondaryRegion,
}
