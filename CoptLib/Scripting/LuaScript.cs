using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Scripting.Typed;
using NLua;
using System.Linq;

namespace CoptLib.Scripting;

public class LuaScript : IScript<object?>
{
    public const string TYPE_ID = "lua";

    public LuaScript(string scriptBody)
    {
        ScriptBody = scriptBody;
    }

    public string TypeId => TYPE_ID;

    public string ScriptBody { get; private set; }

    public object? Output { get; private set; }

    public bool Evaluated { get; private set; }

    public object? Execute(ILoadContext? context)
    {
        if (Evaluated)
            return Output;

        using Lua state = new();
        state.LoadCLRPackage();
        state.DoString("import ('CoptLib', 'CoptLib')");
        state.DoString("import ('CoptLib', 'CoptLib.Models')");
        state.DoString("import ('CoptLib', 'CoptLib.Text')");
        state.DoString("import ('NodaTime', 'NodaTime')");

        state["context"] = context;
        state["dayOfWeek"] = () => (int)context!.CurrentDate.DayOfWeek;

        Output = state.DoString(ScriptBody)?.FirstOrDefault();
        Evaluated = true;

        return Output;
    }

    /// <summary>
    /// Registers a built-in implementation of <see cref="LuaScript"/> with <see cref="ScriptingEngine"/>.
    /// </summary>
    /// <remarks>
    /// No action will be taken if a script type with the same ID is already registered.
    /// </remarks>
    public static void Register()
    {
        if (!ScriptingEngine.ScriptFactories.ContainsKey(TYPE_ID))
            ScriptingEngine.ScriptFactories.Add(TYPE_ID, (b, _, p) => new LuaScript(b));
    }
}
