using System;
using CoptLib.IO;
using CoptLib.Scripting;

namespace CoptLib.Models.Sequences;

public abstract record SequenceNode(int Id, string DocumentKey)
{
    public abstract int? NextNode(LoadContextBase context);
}

public sealed record NullSequenceNode() : SequenceNode(-1, string.Empty)
{
    public static readonly NullSequenceNode Default = new();
    
    public override int? NextNode(LoadContextBase context) => null;
}

public sealed record DelegateSequenceNode(int Id, string DocumentKey, Func<int, LoadContextBase, int?> ToNextNode)
    : SequenceNode(Id, DocumentKey)
{
    public override int? NextNode(LoadContextBase context) => ToNextNode(Id, context);
}

public record ScriptedSequenceNode<T>(int Id, string DocumentKey, ICommandOutput<int?> NextDocCommand)
    : SequenceNode(Id, DocumentKey)
{
    public override int? NextNode(LoadContextBase context)
    {
        NextDocCommand.Execute(context);
        return NextDocCommand.Output;
    }
}