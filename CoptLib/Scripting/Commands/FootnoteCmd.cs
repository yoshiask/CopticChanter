using CoptLib.Extensions;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Text;

namespace CoptLib.Scripting.Commands;

public class FootnoteCmd : TextCommandBase
{
    private readonly IDefinition _footnoteDef;
    private readonly IDefinition _annotatedDef;
    
    public FootnoteCmd(string cmd, InlineCommand inline, IDefinition[] parameters)
        : base(cmd, inline, parameters)
    {
        _annotatedDef = Parameters[0];
        _footnoteDef = Parameters[1];
    }

    public Inline FootnoteInline { get; protected set; }
    
    public int FootnoteNumber { get; protected set; }
    
    protected override IDefinition? ExecuteInternal(ILoadContext? context)
    {
        const string lastNumberKey = $"__{nameof(FootnoteCmd)}-lastNumber";
        if (!context!.TryLookupDefinition(lastNumberKey, out var lastNumberDef, Inline.DocContext))
            lastNumberDef = new Definition<int>(0, lastNumberKey);
        FootnoteNumber = ++((Definition<int>) lastNumberDef).Value;
        context.AddDefinition(lastNumberDef, Inline.DocContext);

        Inline[] outputLines =
        {
            ToInline(_annotatedDef),
            new Run($"[{FootnoteNumber}]", Inline)
        };
        _annotatedDef.RegisterReference(outputLines[0]);
        Span outputSpan = new(outputLines, null);
        
        FootnoteInline = ToInline(_footnoteDef);
        return outputSpan;
    }

    protected static Inline ToInline(IDefinition def)
    {
        if (def is Inline inline)
        {
            return inline;
        }
        if (def is IContent content)
        {
            Span contentSpan = new(content.Inlines, null);
            content.RegisterReference(contentSpan);
            return contentSpan;
        }

        Run defaultRun = new(def.ToString(), null);
        def.RegisterReference(defaultRun);
        return defaultRun;
    }
}