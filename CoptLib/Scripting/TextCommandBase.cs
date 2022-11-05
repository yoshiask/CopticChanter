using CoptLib.Models;

namespace CoptLib.Scripting
{
    /// <summary>
    /// Represents a command that was embedded in an <see cref="IContent"/> 
    /// </summary>
    public abstract class TextCommandBase
    {
        public TextCommandBase(string name, IContent content, int startIndex, string[] parameters)
        {
            Name = name;
            SourceContent = content;
            StartIndex = startIndex;
            Parameters = parameters;
        }

        /// <summary>
        /// The name of the command.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The parameters used to call the command.
        /// </summary>
        public string[] Parameters { get; }

        /// <summary>
        /// The plain text content of the command, if any.
        /// </summary>
        /// <remarks>
        /// Set to <see langword="null"/> to leave the command in
        /// the source text, or <see cref="string.Empty"/> to remove
        /// it.
        /// </remarks>
        public string Text { get; protected set; }

        /// <summary>
        /// The content that contained the command.
        /// </summary>
        public IContent SourceContent { get; }

        /// <summary>
        /// The document context.
        /// </summary>
        public Doc DocContext { get; }

        /// <summary>
        /// The zero-based starting character position of <see cref="Text"/>
        /// in the parent <see cref="IContent"/>.
        /// </summary>
        /// <remarks>
        /// This property is often used in conjunction with <see cref="string.Substring(int, int)"/>,
        /// where the <c>length</c> parameter is <c>Text.Length</c>.
        /// </remarks>
        public int StartIndex { get; }
    }
}
