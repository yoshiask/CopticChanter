namespace CoptLib.Models
{
    public class Comment : Paragraph
    {
        public Comment(IDefinition parent) : base(parent)
        {
        }

        /// <summary>
        /// The type of comment.
        /// </summary>
        /// <remarks>
        /// <b>Examples:</b>
        /// <list type="bullet">
        ///     <item>Tune</item>
        ///     <item>Description</item>
        ///     <item>Explanation</item>
        ///     <item>Commentary</item>
        /// </list>
        /// </remarks>
        public string Type { get; set; }
    }
}
