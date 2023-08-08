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
        set
        {
            if (_sourceText != value)
            {
                FontHandled = false;
                CommandsHandled = false;
                Inlines = new()
                {
                    new Run(value, this)
                };
            }

            _sourceText = value;
        }
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