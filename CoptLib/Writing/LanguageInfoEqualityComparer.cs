using System.Collections.Generic;

namespace CoptLib.Writing;

public class LanguageInfoEqualityComparer : EqualityComparer<LanguageInfo>
{
    public static readonly LanguageInfoEqualityComparer Strict = new(LanguageEquivalencyOptions.Strict);
    public static readonly LanguageInfoEqualityComparer StrictWithWild = new(LanguageEquivalencyOptions.StrictWithWild);

    public LanguageEquivalencyOptions Options { get; }

    public LanguageInfoEqualityComparer(LanguageEquivalencyOptions options)
    {
        Options = options;
    }

    public override bool Equals(LanguageInfo x, LanguageInfo y) => LanguageInfo.IsEquivalentTo(x, y, Options);

    public override int GetHashCode(LanguageInfo obj) => 1; // Return a constant to force use of Equals
}
