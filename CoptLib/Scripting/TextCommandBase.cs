using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;

namespace CoptLib.Scripting
{
    /// <summary>
    /// Represents a command that was embedded in a <see cref="Models.Text.Run"/>.
    /// </summary>
    public abstract class TextCommandBase : ICommandOutput
    {
        public TextCommandBase(string name, Run run, IDefinition[] parameters)
        {
            Name = name;
            Run = run;
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
        public Run Run { get; }

        public Doc DocContext { get; }

        public IDefinition Output { get; protected set; }

        protected void HandleOutput()
        {
            if (Output is TranslationCollection defCol)
            {
                KnownLanguage lang = KnownLanguage.Default;
                if (Run is IMultilingual multi && multi.Language != null)
                    lang = multi.Language.Known;
                else if (Run.Parent is IMultilingual parentMulti)
                    lang = parentMulti.Language.Known;

                Output = defCol[lang];
            }
        }
    }
}
