using CoptLib.Models;
using CoptLib.Writing;

namespace CoptLib.Scripting
{
    /// <summary>
    /// Represents a text command that outputs an <see cref="IDefinition"/>.
    /// </summary>
    public interface ITextCommandDefOut
    {
        /// <summary>
        /// The output as a definition.
        /// </summary>
        public IDefinition Output { get; }
    }

    public static class ITextCommandDefOutExtensions
    {
        public static (IDefinition output, string text) HandleOutput<TCmd>(this TCmd cmd)
            where TCmd : TextCommandBase, ITextCommandDefOut
        {
            var output = cmd.Output;
            string text = null;

            if (output is TranslationCollection defCol)
            {
                Language lang = Language.Default;
                if (cmd.SourceContent is IMultilingual multi)
                    lang = multi.Language;
                else if (cmd.SourceContent.Parent is IMultilingual parentMulti)
                    lang = parentMulti.Language;

                output = defCol[lang];
            }
            if (output is IContent defContent)
                text = defContent.Text;

            return (output, text);
        }
    }
}
