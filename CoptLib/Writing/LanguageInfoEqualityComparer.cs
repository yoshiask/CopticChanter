using System.Collections.Generic;

namespace CoptLib.Writing;

public class LanguageInfoEqualityComparer : EqualityComparer<LanguageInfo>
{
    public LanguageEquivalencyOptions Options { get; }

    public LanguageInfoEqualityComparer(LanguageEquivalencyOptions options = LanguageEquivalencyOptions.Strict)
    {
        Options = options;
    }

    public override bool Equals(LanguageInfo x, LanguageInfo y) => x.IsEquivalentTo(y, Options);

    public override int GetHashCode(LanguageInfo obj) => obj.GetHashCode();
}
