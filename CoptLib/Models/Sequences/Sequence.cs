using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CoptLib.IO;
using OwlCore.Storage;

namespace CoptLib.Models.Sequences;

public class Sequence : IContextualLoad
{
    public Sequence(int rootNodeId, ILoadContext context, string? key = null)
    {
        RootNodeId = rootNodeId;
        Context = context;
        Key = key;
        Nodes = new(node => node.Id);
    }

    public string? Key { get; set; }

    public string? Name { get; set; }

    [NotNull]
    public ILoadContext? Context { get; set; }

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
    public IAsyncEnumerable<SequenceNode> EnumerateNodes()
        => new SequenceEnumerable(Nodes[RootNodeId], Context, ResolveNodeAsync);

    protected virtual Task<SequenceNode> ResolveNodeAsync(int nodeId) => Task.FromResult(Nodes[nodeId]);
}

public delegate ValueTask<Doc?> AsyncDocResolver(string key);
public delegate Doc? DocResolver(string key);

public static class SequenceEx
{
    /// <summary>
    /// Creates an <see cref="IAsyncEnumerable{T}"/> that steps through
    /// documents in the sequence in order, with documents resolved from
    /// the sequence's load context.
    /// </summary>
    public static IAsyncEnumerable<Doc> EnumerateDocs(this Sequence seq)
        => seq.EnumerateDocs(key => ContextCachedDocResolver(key, seq.Context));
    
    /// <summary>
    /// Creates an <see cref="IAsyncEnumerable{T}"/> that steps through
    /// documents in the sequence in order.
    /// </summary>
    public static IAsyncEnumerable<Doc> EnumerateDocs(this Sequence seq, DocResolver resolver) =>
        seq.EnumerateNodes()
            .Select(n => n.DocumentKey is not null ? resolver(n.DocumentKey) : null)
            .Where(d => d is not null)
            .OfType<Doc>();
    
    public static IAsyncEnumerable<Doc> EnumerateDocs(this Sequence seq, AsyncDocResolver resolver) =>
        seq.EnumerateNodes()
            .SelectAwait<SequenceNode, Doc?>(n => n.DocumentKey is not null
                ? resolver(n.DocumentKey) : new ValueTask<Doc?>((Doc?)null))
            .Where(d => d is not null)
            .OfType<Doc>();

    public static Doc? ContextCachedDocResolver(string key, ILoadContext context)
        => context.LookupDefinition(key) as Doc;

    public static AsyncDocResolver LazyLoadedDocResolverFactory(
        ILoadContext context, IAsyncEnumerable<IFolder> setFolders)
    {
        int loadedSetCount = 0;
        return async key =>
        {
            if (context.TryLookupDefinition(key, out var def) && def is Doc cachedDoc)
                return cachedDoc;
            
            // Check for the document in the provided sets, loading the index if necessary
            await foreach (var setFolder in setFolders.Skip(loadedSetCount++))
            {
                // This will load every doc in a set. Technically, we can use the set index to first check if the
                // set contains the doc we're looking for.
                DocSetReader setReader = new(setFolder, context);
                await setReader.ReadDocs();

                var doc = setReader.Set.IncludedDocs.FirstOrDefault(d => d.Key == key);
                if (doc is not null)
                    return doc;
            }

            return null;
        };
    }
}