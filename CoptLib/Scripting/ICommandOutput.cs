using System.Diagnostics.CodeAnalysis;
using CoptLib.Models;

namespace CoptLib.Scripting;

/// <summary>
/// Represents a command (either a script or text command) that
/// can output an <see cref="IDefinition"/>.
/// </summary>
public interface ICommandOutput<out TReturn>
{
    /// <summary>
    /// The output of the command.
    /// </summary>
    /// <remarks>
    /// Set to <see langword="null"/> to remove the command
    /// from the source text.
    /// </remarks>
    public TReturn? Output { get; }

    /// <summary>
    /// Whether the command has been evaluated.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Output))]
    public bool Evaluated { get; }
}