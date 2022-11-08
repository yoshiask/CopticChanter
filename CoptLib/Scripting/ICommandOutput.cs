using CoptLib.Models;

namespace CoptLib.Scripting
{
    /// <summary>
    /// Represents a command (either a script or text command) that
    /// can output an <see cref="IDefinition"/>.
    /// </summary>
    public interface ICommandOutput
    {
        /// <summary>
        /// The output definition of the command.
        /// </summary>
        /// <remarks>
        /// Set to <see langword="null"/> to remove the command
        /// from the source text.
        /// </remarks>
        public IDefinition Output { get; }

        /// <summary>
        /// The document context.
        /// </summary>
        public Doc DocContext { get; }
    }
}
