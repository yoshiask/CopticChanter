using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

namespace CoptLib.Models.Text;

/// <summary>
/// Groups other <see cref="Inline"/> content elements.
/// </summary>
public class Span : Inline
{
    private InlineCollection _inlines;
    public Span(IEnumerable<Inline> inlines, IDefinition? parent) : this(new InlineCollection(inlines), parent) { }

    public Span(InlineCollection inlines, IDefinition? parent) : base(parent)
    {
        Inlines = inlines;
    }

    public InlineCollection Inlines
    {
        get => _inlines;
        set
        {
            foreach (var inline in value)
                inline.Parent = this;
            _inlines = value;
        }
    }

    public override void HandleFont()
    {
        if (FontHandled)
            return;

        Guard.IsNotNull(Inlines);

        foreach (Inline inline in Inlines)
            inline.HandleFont();

        FontHandled = true;
    }

    public override string ToString() => Inlines.ToString();
}