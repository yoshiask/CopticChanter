using CommunityToolkit.Diagnostics;
using System.Collections.Generic;

namespace CoptLib.Models.Text
{
    /// <summary>
    /// Groups other <see cref="Inline"/> content elements.
    /// </summary>
    public class Span : Inline
    {
        public Span(IDefinition parent) : base(parent) { }

        public Span(IEnumerable<Inline> inlines, IDefinition parent) : this(new InlineCollection(inlines), parent) { }

        public Span(InlineCollection inlines, IDefinition parent) : base(parent)
        {
            Inlines = inlines;
        }

        public InlineCollection Inlines { get; set; }

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
}
