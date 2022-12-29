using System.Collections.Generic;

namespace CoptLib.Models.Text
{
    public class InlineCollection : List<Inline>
    {
        public InlineCollection() : base() { }

        public InlineCollection(IEnumerable<Inline> inlines) : base(inlines) { }

        /// <summary>
        /// Appends the contents of the given <see cref="Inline"/> to the current collection.
        /// </summary>
        /// <remarks>
        /// If <paramref name="inline"/> is a <see cref="Run"/>, this is equivalent to <see cref="List{T}.Add(T)"/>.<br/>
        /// If <paramref name="inline"/> is a <see cref="Span"/>, this is roughly equivalent to <see cref="List{T}.AddRange(IEnumerable{T})"/>.
        /// </remarks>
        public void Append(Inline inline)
        {
            if (inline is Run run)
            {
                Add(run);
            }
            else if (inline is Span span)
            {
                foreach (Inline spanInline in span.Inlines)
                {
                    // Ensure the correct parent is being used,
                    // since we're effecively going up one level
                    spanInline.Parent = span.Parent;

                    Add(spanInline);
                }
            }
        }

        /// <summary>
        /// Appends each <see cref="Inline"/> in order.
        /// </summary>
        /// <returns>
        /// A plain string representing this collection.
        /// </returns>
        public override string ToString() => string.Join(string.Empty, this);
    }
}
