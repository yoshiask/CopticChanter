using CoptLib.Models.Text;
using CoptLib.Scripting;
using System.Collections.Generic;

namespace CoptLib.Models
{
    /// <summary>
    /// An object with complex text content that may contain inline commands.
    /// </summary>
    public interface IContent : IDefinition, ISupportsTextCommands
    {
        /// <summary>
        /// The original text, containing any commands or format specifiers.
        /// </summary>
        string SourceText { get; set; }

        /// <summary>
        /// A collection of <see cref="Run"/>s containing the rich content
        /// with important metadata.
        /// </summary>
        InlineCollection Inlines { get; set; }

        /// <summary>
        /// Flattens <see cref="Inlines"/> into a single plain-text string.
        /// </summary>
        string GetText();
    }
}
