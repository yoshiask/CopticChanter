﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CoptLib.IO;

namespace CoptLib.Models.Sequences;

public class Sequence : IContextualLoad
{
    public Sequence(int rootNodeId, LoadContextBase context, string? key = null)
    {
        RootNodeId = rootNodeId;
        Context = context;
        Key = key;
        Nodes = new(node => node.Id);
    }
    
    public string? Key { get; set; }
    
    public string? Name { get; set; }
    
    [NotNull]
    public LoadContextBase? Context { get; set; }
    
    /// <summary>
    /// The identifier of the node to always start at.
    /// </summary>
    public int RootNodeId { get; }
    
    /// <summary>
    /// A collection of all nodes in the graph.
    /// </summary>
    public ElementKeyedDictionary<int, SequenceNode> Nodes { get; }

    /// <summary>
    /// Creates an <see cref="IAsyncEnumerable{T}"/> that steps through
    /// the sequence in order.
    /// </summary>
    /// <returns></returns>
    public IAsyncEnumerable<SequenceNode> EnumerateNodes()
        => new SequenceEnumerable(Nodes[RootNodeId], Context, ResolveNodeAsync);

    protected virtual Task<SequenceNode> ResolveNodeAsync(int nodeId) => Task.FromResult(Nodes[nodeId]);
}