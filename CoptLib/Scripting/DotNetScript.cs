using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Scripting.Typed;
using CSScriptLib;

namespace CoptLib.Scripting;

public class DotNetScript : ScriptBase<object?>
{
    public const string TYPE_ID = "csharp";
    private const string CommonUsings = "using CoptLib;\r\nusing CoptLib.IO;\r\nusing CoptLib.Models;\r\nusing CoptLib.Models.Text;\r\nusing CoptLib.Writing;\r\nusing NodaTime;\r\n";

    public DotNetScript(string scriptBody, IDefinition? parent = null) : base(scriptBody)
    {
        Parent = parent;
    }

    public override string TypeId { get; protected set; } = "csharp";
    
    protected IScriptImplementation? Implementation { get; set; }

    public override object? Execute(ILoadContext? context)
    {
        Implementation = GetImplementation(ScriptBody);
        return ExecuteInternal(context ?? DocContext?.Context);
    }

    protected override object? ExecuteInternal(ILoadContext? context)
        => Implementation!.Execute(context);

    protected virtual IScriptImplementation GetImplementation(string code)
        => CSScript.Evaluator.LoadMethod<IScriptImplementation>(GetSourceCode(code));

    /// <summary>
    /// Registers built-in implementations of <see cref="DotNetScript"/> with <see cref="ScriptingEngine"/>.
    /// </summary>
    /// <remarks>
    /// No action will be taken if a script type with the same ID is already registered.
    /// </remarks>
    public static void Register()
    {
        if (!ScriptingEngine.ScriptFactories.ContainsKey(TYPE_ID))
        {
            ScriptingEngine.ScriptFactories.Add(TYPE_ID, (b, _, p) =>
            {
                IDefinition? parent = null;
                if (p is not null && p.TryGetValue("parent", out var parentObj))
                    parent = parentObj as IDefinition;

                return new DotNetScript(b, parent);
            });
        }
        
        if (!ScriptingEngine.ScriptFactories.ContainsKey(DotNetDefinitionScript.TYPE_ID))
        {
            ScriptingEngine.ScriptFactories.Add(DotNetDefinitionScript.TYPE_ID, (b, _, p) =>
            {
                IDefinition? parent = null;
                if (p is not null && p.TryGetValue("parent", out var parentObj))
                    parent = parentObj as IDefinition;

                return new DotNetDefinitionScript(b, parent);
            });
        }
    }

    private static string GetSourceCode(string code)
    {
        return CommonUsings
           + "public object? Execute(ILoadContext? context) {\r\n"
           + code
           + "\r\n}";
    }
}

public class DotNetScript<TOut> : DotNetScript, ICommandOutput<TOut?>
{
    public DotNetScript(string scriptBody, IDefinition? parent = null) : base(scriptBody, parent)
    {
    }

    public new TOut? Output { get; protected set; }
    
    public new TOut? Execute(ILoadContext? context)
    {
        var output = base.Execute(context);
        if (output is null)
            return default;
        
        return Output = (TOut)output;
    }
}
