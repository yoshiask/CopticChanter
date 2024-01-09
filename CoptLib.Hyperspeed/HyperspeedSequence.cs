using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using CoptLib.Hyperspeed.IO;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Sequences;

namespace CoptLib.Hyperspeed;

/// <summary>
/// An <see cref="IReadOnlySequence"/> optimized for enumerating nodes or documents without
/// having to parse every node.
/// </summary>
public class HyperspeedSequence : Definition, IReadOnlySequence, IDisposable
{
    private readonly IReadOnlyDictionary<int, long> _offsetMap;
    private readonly Stream _nodesStream;
    private readonly HyperspeedBinaryReader _reader;

    public HyperspeedSequence(string? key, string? name, ILoadContext context, int rootNodeId,
        IReadOnlyDictionary<int, long> offsetMap, Stream nodesStream)
    {
        _offsetMap = offsetMap;
        _nodesStream = nodesStream;
        _reader = new(nodesStream, context);
        
        Key = key;
        Name = name;
        Context = context;
        RootNodeId = rootNodeId;
    }

    public string? Key { get; set; }
    
    public string? Name { get; }
    
    [NotNull]
    public ILoadContext? Context { get; set; }
    
    public int RootNodeId { get; }
    
    public async IAsyncEnumerable<SequenceNode> EnumerateNodes()
    {
        var currentNode = _reader.ReadSequenceNode(this);
        while (currentNode != NullSequenceNode.Default)
        {
            // Read next node
            var nextNodeId = await Task.Run(() => currentNode.NextNode(_reader.Context!));
            if (nextNodeId is null)
                yield break;

            var nextNodeOffset = _offsetMap[nextNodeId.Value];
            _nodesStream.Position = nextNodeOffset;
            yield return _reader.ReadSequenceNode(this);
        }
    }

    public void Dispose() => _reader.Dispose();
}