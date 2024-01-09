using System;
using CoptLib.IO;
using CoptLib.Scripting;

namespace CoptLib.Models.Sequences;

public abstract record SequenceNode(int Id, string? DocumentKey)
{
    public abstract int? NextNode(ILoadContext context);
}

public record ConstantSequenceNode(int Id, string? DocumentKey, int? NextNodeId) : SequenceNode(Id, DocumentKey)
{
    public override int? NextNode(ILoadContext context) => NextNodeId;
}

public sealed record NullSequenceNode(int Id) : ConstantSequenceNode(Id, null, null)
{
    public static readonly NullSequenceNode Default = new(-1);
}

public sealed record EndSequenceNode(int Id, string? DocumentKey) : ConstantSequenceNode(Id, DocumentKey, null);

public sealed record DelegateSequenceNode(int Id, string? DocumentKey, Func<int, ILoadContext, int?> ToNextNode)
    : SequenceNode(Id, DocumentKey)
{
    public override int? NextNode(ILoadContext context) => ToNextNode(Id, context);
}

public record ScriptedSequenceNode(int Id, string? DocumentKey, ICommandOutput<object> NextDocCommand)
    : SequenceNode(Id, DocumentKey)
{
    public override int? NextNode(ILoadContext context)
    {
        NextDocCommand.Execute(context);

        return NextDocCommand.Output is null
            ? null
            : Convert.ToInt32(NextDocCommand.Output);
    }
}