using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoptLib.IO;

namespace CoptLib.Models.Sequences;

public class SequenceEnumerable : IAsyncEnumerable<SequenceNode>
{
    private readonly SequenceNode _rootNode;
    private readonly ILoadContext _context;
    private readonly Func<int, Task<SequenceNode>> _resolver;

    public SequenceEnumerable(SequenceNode rootNode, ILoadContext context, Func<int, Task<SequenceNode>> resolver)
    {
        _rootNode = rootNode;
        _context = context;
        _resolver = resolver;
    }
    
    public IAsyncEnumerator<SequenceNode> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new SequenceEnumerator(_rootNode, _context, _resolver);

    private class SequenceEnumerator : IAsyncEnumerator<SequenceNode>
    {
        private readonly SequenceNode _rootNode;
        private readonly ILoadContext _context;
        private readonly Func<int, Task<SequenceNode>> _resolver;
        
        private bool _isFirstIteration = true;

        public SequenceEnumerator(SequenceNode rootNode, ILoadContext context, Func<int, Task<SequenceNode>> resolver)
        {
            _rootNode = rootNode;
            _context = context;
            _resolver = resolver;
        }
        
        public async ValueTask DisposeAsync() {}

        public async ValueTask<bool> MoveNextAsync()
        {
            if (_isFirstIteration)
            {
                _isFirstIteration = false;
                Current = _rootNode;
                return Current != NullSequenceNode.Default;
            }

            var nextId = await Task.Run(() => Current.NextNode(_context));
            if (nextId is null)
                return false;

            Current = await _resolver(nextId.Value);
            return true;
        }

        public SequenceNode Current { get; private set; } = NullSequenceNode.Default;
    }
}