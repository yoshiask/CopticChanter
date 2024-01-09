using System;
using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using System.Linq;
using CoptLib.IO;

namespace CoptLib.Scripting.Commands;

public class DefinitionCmd : TextCommandBase
{
    private readonly string _definitionKey;
    
    public DefinitionCmd(string name, InlineCommand inline, IDefinition[] parameters)
        : base(name, inline, parameters)
    {
        _definitionKey = Parameters.FirstOrDefault()!.ToString();
    }

    protected override IDefinition ExecuteInternal(ILoadContext? context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        var def = context.LookupDefinition(_definitionKey, Inline.DocContext);
        if (def is null)
            throw new Exception($"No definition with key '{_definitionKey}' was found.");

        // Register the current inline with the referenced definition
        def.RegisterReference(Inline);

        return def;
    }
}