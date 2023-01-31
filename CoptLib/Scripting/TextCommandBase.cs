using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;

namespace CoptLib.Scripting
{
    /// <summary>
    /// Represents a command that was embedded in a <see cref="Run"/>.
    /// </summary>
    public abstract class TextCommandBase : ICommandOutput<IDefinition>
    {
        public TextCommandBase(string name, InlineCommand inline, IDefinition[] parameters)
        {
            Name = name;
            Inline = inline;
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
        public InlineCommand Inline { get; }

        public Doc DocContext { get; }

        public IDefinition Output { get; set; }

        protected void HandleOutput()
        {
            if (Output is ITranslationLookup defLookup)
            {
                KnownLanguage lang = Inline.GetLanguage().Known;
                Output = defLookup.GetByLanguage<IMultilingual>(lang) as IDefinition;
            }
        }
    }
}
