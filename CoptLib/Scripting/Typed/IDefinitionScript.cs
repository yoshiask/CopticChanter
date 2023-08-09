using CoptLib.IO;
using CoptLib.Models;

namespace CoptLib.Scripting.Typed;

/// <summary>
/// Represents a script implementation that outputs any <see cref="object"/>.
/// </summary>
public interface IScriptImplementation<out TOut>
{
    TOut Execute(LoadContextBase? context);
}

/// <summary>
/// Represents a script implementation that outputs an <see cref="IDefinition"/>.
/// </summary>
public interface IDefinitionScript<out TDef> : IScriptImplementation<TDef>
    where TDef : IDefinition
{
}

public abstract class DefinitionScriptBase : IDefinitionScript<IDefinition>
{
    public ICommandOutput<IDefinition>? Parent { get; set; }

    public abstract IDefinition Execute(LoadContextBase? context);
}
