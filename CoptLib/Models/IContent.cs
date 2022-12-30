using CoptLib.Models.Text;
using CoptLib.Scripting;
using System.Collections.Generic;

namespace CoptLib.Models
{
    /// <summary>
    /// An object with content that may contain text commands.
    /// </summary>
    public interface IContent : IDefinition
    {
        /// <summary>
        /// The original text, containing any commands or format specifiers.
        /// </summary>
        string SourceText { get; set; }

        /// <summary>
        /// Whether text commands in the content have been
        /// parsed and executed.
        /// </summary>
        bool CommandsHandled { get; set; }

        /// <summary>
        /// A collection of <see cref="Run"/>s containing the rich content
        /// with important metadata.
        /// </summary>
        InlineCollection Inlines { get; set; }

        /// <summary>
        /// A list of commands parsed from <see cref="SourceText"/>.
        /// </summary>
        /// <remarks>
        /// This property is populated by calling <see cref="HandleCommands"/>.
        /// </remarks>
        List<TextCommandBase> Commands { get; set; }

        /// <summary>
        /// Parses the <see cref="SourceText"/> for commands, storing the results in
        /// <see cref="Inlines"/> and <see cref="Commands"/>.
        /// </summary>
        void HandleCommands();

        /// <summary>
        /// Flattens <see cref="Inlines"/> into a single plain-text string.
        /// </summary>
        string GetText();
    }
}
