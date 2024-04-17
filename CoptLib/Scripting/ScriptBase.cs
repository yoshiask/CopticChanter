using CoptLib.IO;
using CoptLib.Models;

namespace CoptLib.Scripting;

public abstract class ScriptBase<T>(string scriptBody) : Definition, IScript<T>
{
    public T? Output { get; private set; }
    public bool Evaluated { get; private set; }
    
    public virtual T? Execute(ILoadContext? context)
    {
        if (Evaluated)
            return Output;
        
        Output = ExecuteInternal(context);
        Evaluated = true;
        return Output;
    }

    protected abstract T? ExecuteInternal(ILoadContext? context);

    public string ScriptBody { get; } = scriptBody;
    public abstract string TypeId { get; protected set; }
}