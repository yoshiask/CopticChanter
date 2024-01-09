using CoptLib.Models.Text;
using CoptLib.Scripting;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace CoptLib.Models;

[XmlRoot("String")]
public class SimpleContent : Definition, IContent
{
    private string _sourceText;

    public SimpleContent(string? sourceText, IDefinition? parent)
    {
        SourceText = sourceText ?? string.Empty;
        Parent = parent;
        DocContext = parent?.DocContext;
    }

    public string SourceText
    {
        get => _sourceText;
        
        [MemberNotNull(nameof(_sourceText))]
        [MemberNotNull(nameof(Inlines))]
        [MemberNotNull(nameof(Commands))]
        set => _sourceText = ContentHelper.UpdateSourceText(this, value);
    }

    public bool CommandsHandled { get; set; }

    public InlineCollection Inlines { get; set; }

    public List<TextCommandBase> Commands { get; set; }

    public string GetText() => ContentHelper.GetText(this);

    public void HandleCommands() => ContentHelper.HandleCommands(this);

    public override string ToString() => GetText();
}