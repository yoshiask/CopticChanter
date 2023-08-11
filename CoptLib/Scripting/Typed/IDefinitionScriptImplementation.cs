using CoptLib.IO;
using CoptLib.Models;

namespace CoptLib.Scripting.Typed;

/// <summary>
/// Represents a script implementation that outputs an <see cref="IDefinition"/>.
/// </summary>
public interface IDefinitionScriptImplementation<out TDef> : IScriptImplementation<TDef>
    where TDef : IDefinition
{
}

public abstract class DefinitionScriptImplementationBase : IDefinitionScriptImplementation<IDefinition>
{
    public ICommandOutput<IDefinition>? Parent { get; set; }

    public abstract IDefinition Execute(LoadContextBase? context);
}