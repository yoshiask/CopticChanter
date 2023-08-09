using System.Linq;
using System.Threading.Tasks;
using CoptLib.IO;
using CoptLib.Models.Sequences;
using Xunit;

namespace CoptTest;

public class Sequences
{
    [Fact]
    public async Task TraverseSequenceTree()
    {
        const int nodeCount = 10;
        
        int? ToNextNode(int currentId, LoadContextBase context)
            => currentId + 1 == nodeCount ? null : currentId + 1;

        var nodes = Enumerable.Range(0, nodeCount)
            .Select(i => new DelegateSequenceNode(i, "Doc" + i, ToNextNode))
            .ToArray();

        SequenceEnumerable sequence = new(nodes[0], new LoadContext(),
            i => Task.Run<SequenceNode>(() => nodes[i]));

        int currentNodeIdEx = 0;
        await foreach (var node in sequence)
        {
            Assert.Equal(currentNodeIdEx, node.Id);
            Assert.Equal("Doc" + currentNodeIdEx, node.DocumentKey);
            ++currentNodeIdEx;
        }
        
        Assert.Equal(nodeCount, currentNodeIdEx);
    }
}