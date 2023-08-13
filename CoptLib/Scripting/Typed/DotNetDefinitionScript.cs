using CoptLib.IO;
using CoptLib.Models;

namespace CoptLib.Scripting.Typed;

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