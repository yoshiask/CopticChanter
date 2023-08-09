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

public class DotNetDefinitionScript : DotNetScript<DefinitionScriptImplementationBase, IDefinition>
{
    public DotNetDefinitionScript(string scriptBody, IDefinition? parent = null) : base(scriptBody, parent)
    {
    }

    protected override IDefinition ExecuteInternal(LoadContextBase? context)
    {
        var output = Implementation!.Execute(DocContext?.Context);
        output.DocContext = DocContext;
        output.Parent = this;
        return output;
    }
}
