using CoptLib.Writing;
using System;

namespace CoptLib.Models.Text
{
    public class Run : Inline
    {
        public Run(IDefinition parent) : base(parent) { }

        public Run(ReadOnlySpan<char> text, IDefinition parent) : this(new string(text.ToArray()), parent)
        {
        }

        public Run(string text, IDefinition parent) : this(parent)
        {
            Text = text;
        }

        public string Text { get; set; }

        public override void HandleFont()
        {
            if (!FontHandled && CopticFont.TryFindFont(Font, out var font))
            {
                Text = font.Convert(Text);
                FontHandled = true;
            }
        }

        public override string ToString() => Text;

        public override bool Equals(object obj) => obj is Run other && Text == other.Text;

        public override int GetHashCode() => Text?.GetHashCode() ?? 0;
    }
}
