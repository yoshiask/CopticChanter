using System.Collections.Generic;
using CoptLib.IO;

namespace CoptLib.Models.Sequences;

public interface ISequence : IReadOnlySequence
{
    /// <summary>
    /// A collection of all nodes in the graph.
    /// </summary>
    public ElementKeyedDictionary<int, SequenceNode> Nodes { get; }
}

public interface IReadOnlySequence : IContextualLoad
{
    public string? Name { get; }

    /// <summary>
    /// The identifier of the node to always start at.
    /// </summary>
    public int RootNodeId { get; }

    /// <summary>
    /// Creates an <see cref="IAsyncEnumerable{T}"/> that steps through
    /// the sequence in order.
    /// </summary>
    public IAsyncEnumerable<SequenceNode> EnumerateNodes();
}