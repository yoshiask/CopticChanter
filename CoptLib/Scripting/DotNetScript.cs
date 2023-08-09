using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Scripting.Typed;
using CSScriptLib;

namespace CoptLib.Scripting;

public class DotNetScript<TImpl, TOut> : Definition, ICommandOutput<TOut>
    where TImpl : class, IScriptImplementation<TOut>
{
    private const string CommonUsings = "using CoptLib;\r\nusing CoptLib.IO;\r\nusing CoptLib.Models;\r\nusing CoptLib.Models.Text;\r\nusing CoptLib.Writing;\r\nusing NodaTime;\r\n";

    public DotNetScript(string scriptBody, IDefinition? parent = null)
    {
        ScriptBody = scriptBody;
        Parent = parent;
    }
    
    public string ScriptBody { get; }
    
    public TOut? Output { get; protected set; }

    public bool Evaluated { get; protected set; }
    
    protected TImpl? Implementation { get; set; }

    public virtual void Execute(LoadContextBase? context)
    {
        if (Evaluated)
            return;

        Implementation = GetImplementation(ScriptBody);
        Output = ExecuteInternal(context ?? DocContext?.Context);

        Evaluated = true;
    }

    protected virtual TOut ExecuteInternal(LoadContextBase? context)
        => Implementation!.Execute(context);

    protected virtual TImpl GetImplementation(string code)
        => CSScript.Evaluator.LoadMethod<TImpl>(CommonUsings + code);
}

public class DotNetDefinitionScript : DotNetScript<DefinitionScriptBase, IDefinition>
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
