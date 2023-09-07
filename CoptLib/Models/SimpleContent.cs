using CoptLib.Models.Text;
using CoptLib.Scripting;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.Models;

[XmlRoot("String")]
public class SimpleContent : Definition, IContent
{
    private string _sourceText;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public SimpleContent(string? sourceText, IDefinition? parent)
    {
        SourceText = sourceText ?? string.Empty;
        Parent = parent;
        DocContext = parent?.DocContext;
    }
#pragma warning restore CS8618

    public string SourceText
    {
        get => _sourceText;
        set => _sourceText = ContentHelper.UpdateSourceText(this, value);
    }

    public bool CommandsHandled { get; set; }

    public InlineCollection Inlines { get; set; }

    public List<TextCommandBase> Commands { get; set; }

    public string GetText() => ContentHelper.GetText(this);

    public void HandleCommands() => ContentHelper.HandleCommands(this);

    public override string ToString() => GetText();
}