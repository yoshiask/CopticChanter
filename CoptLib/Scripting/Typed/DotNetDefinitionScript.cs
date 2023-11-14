using CoptLib.IO;
using CoptLib.Models;

namespace CoptLib.Scripting.Typed;

public class DotNetDefinitionScript : DotNetScript<IDefinition>
{
    public const string TYPE_ID = "csharp-def";
    
    public DotNetDefinitionScript(string scriptBody, IDefinition? parent = null) : base(scriptBody, parent)
    {
    }

    public override string TypeId => TYPE_ID;

    protected override object ExecuteInternal(ILoadContext? context)
    {
        var output = Implementation!.Execute(context ?? DocContext?.Context) as IDefinition;
        output.DocContext = DocContext;
        output.Parent = this;
        return output;
    }
}