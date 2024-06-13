using System;
using System.Collections.Generic;
using CoptLib.IO;
using DynamicExpresso;

namespace CoptLib.Scripting;

public class DynamicExpressoScript(string typeId, string scriptBody) : ScriptBase<object>(scriptBody)
{
    public const string TYPE_ID = "dynexp";
    public override string TypeId { get; protected set; } = typeId;

    protected override object? ExecuteInternal(ILoadContext? context)
    {
        Interpreter interpreter = new();
        return interpreter.Eval(ScriptBody);
    }

    /// <summary>
    /// Registers a built-in implementation of <see cref="DynamicExpressoScript"/> with <see cref="ScriptingEngine"/>.
    /// </summary>
    /// <remarks>
    /// No action will be taken if a script type with the same ID is already registered.
    /// </remarks>
    public static void Register()
    {
        if (!ScriptingEngine.ScriptFactories.ContainsKey(TYPE_ID))
            ScriptingEngine.ScriptFactories.Add(TYPE_ID, Factory);
    }

    private static DynamicExpressoScript Factory(string b, string? _, IReadOnlyDictionary<string, object>? p)
    {
        return new DynamicExpressoScript(TYPE_ID, b);
    }
}

public class DynamicExpressoScript<TReturn>(string typeId, string scriptBody) : ScriptBase<TReturn>(scriptBody)
{
    public override string TypeId { get; protected set; } = typeId;

    protected override TReturn? ExecuteInternal(ILoadContext? context)
    {
        Interpreter interpreter = new();
        var func = interpreter.ParseAsDelegate<Func<ILoadContext?, TReturn?>>(ScriptBody);
        return func(context);
    }
}