using CoptLib.IO;
using NLua;
using System.Linq;

namespace CoptLib.Scripting;

public class LuaScript(string scriptBody) : ScriptBase<object?>(scriptBody)
{
    public const string TYPE_ID = "lua";
    public override string TypeId { get; protected set; } = TYPE_ID;

    protected override object? ExecuteInternal(ILoadContext? context)
    {
        using Lua state = new();
        state.LoadCLRPackage();
        state.DoString("import ('CoptLib', 'CoptLib')");
        state.DoString("import ('CoptLib', 'CoptLib.Models')");
        state.DoString("import ('CoptLib', 'CoptLib.Text')");
        state.DoString("import ('NodaTime', 'NodaTime')");

        state["context"] = context;
        state["dayOfWeek"] = () => (int)context!.CurrentDate.DayOfWeek;

        return state.DoString(ScriptBody)?.FirstOrDefault();
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
