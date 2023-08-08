using System;
using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using System.Linq;

namespace CoptLib.Scripting.Commands;

public class DefinitionCmd : TextCommandBase
{
    public DefinitionCmd(string name, InlineCommand inline, IDefinition[] parameters)
        : base(name, inline, parameters)
    {
        Parse();
    }

    private void Parse()
    {
        if (DocContext is null)
            throw new NullReferenceException($"A `{nameof(DocContext)}` must be present.");
        
        var defKey = Parameters.FirstOrDefault()!.ToString();
        Output = DocContext.LookupDefinition(defKey);

        if (Output is null)
            throw new Exception($"No definition with key '{defKey}' was found.");

        HandleOutput();

        // Register the current inline with the referenced definition
        Output?.RegisterReference(Inline);

        Evaluated = true;
    }
}