using System;
using CoptLib.IO;
using CoptLib.Scripting;

namespace CoptLib.Models.Sequences;

public abstract record SequenceNode(int Id, string DocumentKey)
{
    public abstract int? NextNode(LoadContextBase context);
}

public record ConstantSequenceNode(int Id, string DocumentKey, int? NextNodeId) : SequenceNode(Id, DocumentKey)
{
    public override int? NextNode(LoadContextBase context) => NextNodeId;
}

public sealed record NullSequenceNode() : ConstantSequenceNode(-1, string.Empty, null)
{
    public static readonly NullSequenceNode Default = new();
}

public sealed record EndSequenceNode(int Id, string DocumentKey) : ConstantSequenceNode(Id, DocumentKey, null)
{
}

public sealed record DelegateSequenceNode(int Id, string DocumentKey, Func<int, LoadContextBase, int?> ToNextNode)
    : SequenceNode(Id, DocumentKey)
{
    public override int? NextNode(LoadContextBase context) => ToNextNode(Id, context);
}

public record ScriptedSequenceNode(int Id, string DocumentKey, ICommandOutput<int?> NextDocCommand)
    : SequenceNode(Id, DocumentKey)
{
    public override int? NextNode(LoadContextBase context)
    {
        NextDocCommand.Execute(context);
        return NextDocCommand.Output;
    }
}