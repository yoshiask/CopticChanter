using System;
using CoptLib.Extensions;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Text;

namespace CoptLib.Scripting.Commands;

public class BooleanCommand(string name, InlineCommand inline, IDefinition[] parameters)
    : TextCommandBase(name, inline, parameters), ICommandOutput<bool>
{
    protected override IDefinition? ExecuteInternal(ILoadContext? context)
    {
        var value = !Name.Equals("false", StringComparison.InvariantCultureIgnoreCase);
        var key = value ? "True" : "False";

        // Attempt to reference existing definition instead of eating memory for a single bool
        if (context?.TryLookupDefinition(key, out var def) ?? false)
        {
            def.RegisterReference(Inline);
            return def;
        }
        return new BooleanDefinition(value, Inline);
    }

    bool ICommandOutput<bool>.Output => (Output as BooleanDefinition)?.Value ?? false;

    bool ICommandOutput<bool>.Execute(ILoadContext? context)
    {
        Execute(context);
        return ((ICommandOutput<bool>)this).Output;
    }
}