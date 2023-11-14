using CoptLib.IO;

namespace CoptLib.Scripting.Typed;

/// <summary>
/// Represents a script implementation that outputs any <see cref="object"/>.
/// </summary>
public interface IScriptImplementation
{
    object Execute(ILoadContext? context);
}