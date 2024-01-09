using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using CoptLib.IO;

namespace CoptLib.Models.Sequences;

public class Sequence : ISequence
{
    public Sequence(int rootNodeId, ILoadContext context, string? key = null)
    {
        RootNodeId = rootNodeId;
        Context = context;
        Key = key;
    }
    
    public string? Key { get; set; }
    
    public string? Name { get; set; }
    
    [NotNull]
    public ILoadContext? Context { get; set; }
    
    public int RootNodeId { get; }
    
    public ElementKeyedDictionary<int, SequenceNode> Nodes { get; } = new(node => node.Id);
    
    public IAsyncEnumerable<SequenceNode> EnumerateNodes()
        => new SequenceEnumerable(Nodes[RootNodeId], Context, ResolveNodeAsync);

    protected Task<SequenceNode> ResolveNodeAsync(int nodeId) => Task.FromResult(Nodes[nodeId]);
}

public static class SequenceExtensions
{
    /// <summary>
    /// Creates an <see cref="IAsyncEnumerable{T}"/> that steps through
    /// documents in the sequence in order.
    /// </summary>
    public static IAsyncEnumerable<Doc> EnumerateDocs(this IReadOnlySequence sequence) => sequence
        .EnumerateNodes()
        .Select(n => n.DocumentKey is not null ? sequence.Context!.LookupDefinition(n.DocumentKey) : null)
        .Where(d => d is not null)
        .OfType<Doc>();
}