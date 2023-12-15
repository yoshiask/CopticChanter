using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Text;
using System.Linq;

namespace CoptLib.Scripting.Commands;

public class LinesCmd : TextCommandBase
{
    public LinesCmd(string name, InlineCommand inline, IDefinition[] parameters)
        : base(name, inline, parameters)
    {
    }

    protected override IDefinition? ExecuteInternal(ILoadContext? context)
    {
        var lineSeparator = Parameters[0].ToString();
        if (string.IsNullOrEmpty(lineSeparator))
        {
            if (context is null || !context.TryLookupDefinition("defaultLineSeparator", out var sepDef))
            {
                lineSeparator = " / ";
            }
            else
            {
                if (sepDef is ITranslationLookup<IMultilingual> sepTranslations)
                    lineSeparator = sepTranslations.GetByLanguage(Inline.Language).ToString();
                else
                    lineSeparator = sepDef.ToString();
            }
        }

        var lines = string.Join(lineSeparator, Parameters.Skip(1).Select(d => d.ToString()));
        return new Run(lines!, Inline);
    }
}
