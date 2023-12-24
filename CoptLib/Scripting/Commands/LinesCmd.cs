using System.Collections.Generic;
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

        return GenerateLines(lineSeparator, Parameters.Skip(1));
    }

    private Span GenerateLines(string lineSeparator, IEnumerable<IDefinition> fragments)
    {
        Span lines = new(new InlineCollection(), Inline);
        
        Run separator = new(lineSeparator, Inline);
        var defs = fragments.ToList();

        for (int i = 0; i < defs.Count; i++)
        {
            var def = defs[i];
            if (def is Inline defInline)
                lines.Inlines.Add(defInline);
            else
                lines.Inlines.Add(new Run(def.ToString(), null));

            if (i < defs.Count - 1)
                lines.Inlines.Add(separator);
        }

        return lines;
    }
}
