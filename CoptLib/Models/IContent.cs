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
        /// Whether the <see cref="SourceText"/> has been parsed.
        /// </summary>
        bool HasBeenParsed { get; }

        /// <summary>
        /// The <see cref="SourceText"/> text, with any commands or non-text strings stripped out.
        /// </summary>
        /// <remarks>
        /// This property is populated by calling <see cref="ParseCommands"/>.
        /// </remarks>
        string Text { get; }

        /// <summary>
        /// A list of commands parsed from <see cref="SourceText"/>.
        /// </summary>
        /// <remarks>
        /// This property is populated by calling <see cref="ParseCommands"/>.
        /// </remarks>
        List<TextCommandBase> Commands { get; }

        /// <summary>
        /// Parses the <see cref="SourceText"/> for commands, storing the results in
        /// <see cref="Text"/> and <see cref="Commands"/>.
        /// </summary>
        void ParseCommands();
    }
}
