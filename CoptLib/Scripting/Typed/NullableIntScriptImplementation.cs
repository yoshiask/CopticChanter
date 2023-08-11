using CoptLib.IO;

namespace CoptLib.Scripting.Typed;

public abstract class NullableIntScriptImplementation : IScriptImplementation<int?>
{
    public abstract int? Execute(LoadContextBase? context);
}