using CoptLib.Models;
using CoptLib.Writing;

namespace CoptLib.Scripting
{
    /// <summary>
    /// Represents a command that was embedded in an <see cref="IContent"/> 
    /// </summary>
    public abstract class TextCommandBase : ICommandOutput
    {
        public TextCommandBase(string name, IContent content, int startIndex, IDefinition[] parameters)
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
        public IDefinition[] Parameters { get; }

        /// <summary>
        /// The content that contained the command.
        /// </summary>
        public IContent SourceContent { get; }

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

        public IDefinition Output { get; protected set; }

        protected void HandleOutput()
        {
            if (Output is TranslationCollection defCol)
            {
                KnownLanguage lang = KnownLanguage.Default;
                if (SourceContent is IMultilingual multi)
                    lang = multi.Language;
                else if (SourceContent.Parent is IMultilingual parentMulti)
                    lang = parentMulti.Language;

                Output = defCol[lang];
            }
        }
    }
}
