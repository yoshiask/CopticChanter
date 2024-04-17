using System.Diagnostics.CodeAnalysis;
using CoptLib.IO;
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

    /// <summary>
    /// Executes the command.
    /// </summary>
    public TReturn? Execute(ILoadContext? context);
}

public sealed record ConstantCommand<TReturn>(TReturn Value) : ICommandOutput<TReturn>
{
    public TReturn? Output => Value;
    public bool Evaluated => true;

    public TReturn? Execute(ILoadContext? context) => Value;
}
