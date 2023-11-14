namespace CoptLib.Scripting;

public interface IScript<out TOut> : ICommandOutput<TOut>
{
    public string ScriptBody { get; }
    
    /// <summary>
    /// The identifier for this type of script.
    /// </summary>
    public string TypeId { get; }
}