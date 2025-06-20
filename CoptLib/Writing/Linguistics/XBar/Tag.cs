using CoptLib.Extensions;
using System.Linq;

namespace CoptLib.Writing.Linguistics.XBar;

public record Tag(PhrasalCategory Category, XBarNodeType Type)
{
    public static Tag Parse(string str)
    {
        var (type, categoryStr) = XBarNodeTypes.ParseSuffix(str);
        var category = PhrasalCategories.Parse(categoryStr);
        return new Tag(category, type);
    }

    public override string ToString() => $"{Category.ToAbbreviation()}{Type.ToSuffix()}";
}
