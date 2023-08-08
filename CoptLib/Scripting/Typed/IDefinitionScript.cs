using CoptLib.Models;

namespace CoptLib.Scripting.Typed;

/// <summary>
/// Represents a script implementation that outputs an <see cref="IDefinition"/>.
/// </summary>
public interface IDefinitionScript<out TDef>
    where TDef : IDefinition
{
    CScript? Parent { get; set; }

    TDef GetDefinition();
}

public abstract class DefinitionScriptBase : IDefinitionScript<IDefinition>
{
    public CScript? Parent { get; set; }

    public abstract IDefinition GetDefinition();
}
