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

    protected override void ExecuteInternal(ILoadContext? context)
    {
        if (context is null)
            throw new ArgumentNullException(nameof(context));

        Output = context.LookupDefinition(_definitionKey, Inline.DocContext);
        if (Output is null)
            throw new Exception($"No definition with key '{_definitionKey}' was found.");

        ApplyNecessaryTransforms();

        // Register the current inline with the referenced definition
        Output.RegisterReference(Inline);
    }
}