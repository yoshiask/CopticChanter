using CoptLib.Models.Text;

namespace CoptLib.Models;

public abstract class Paragraph : ContentPart, IContent
{
    private string _sourceText;

    public Paragraph(IDefinition? parent) : base(parent)
    {
        _sourceText = string.Empty;
    }

    public string SourceText
    {
        get => _sourceText;
        set => _sourceText = ContentHelper.UpdateSourceText(this, value);
    }

    public InlineCollection Inlines { get; set; } = InlineCollection.Empty;

    public override void HandleCommands() => ContentHelper.HandleCommands(this);

    public override void HandleFont()
    {
        ContentHelper.HandleFont(Inlines);
        FontHandled = true;
    }

    public string GetText() => ContentHelper.GetText(this);

    public override string ToString() => GetText();
}