using CoptLib.Extensions;
using System.Linq;

namespace CoptLib.Writing.Linguistics.XBar;

public record Tag(PhrasalCategory Category, XBarNodeType Type)
{
    public static Tag Parse(string str)
    {
        var parts = str.SplitAlongCapitals().ToArray();

        var category = PhrasalCategories.Parse(parts[0]);

        var type = XBarNodeType.PartOfSpeech;
        if (parts.Length >= 2)
            type = XBarNodeTypes.Parse(parts[1]);

        return new(category, type);
    }

    public override string ToString() => $"{Category.ToAbbreviation()}{Type.ToSuffix()}";
}
