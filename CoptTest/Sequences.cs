using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using CoptLib;
using CoptLib.IO;
using CoptLib.Models.Sequences;
using NodaTime;
using Xunit;

namespace CoptTest;

public class Sequences
{
    [Fact]
    public async Task SequenceEnumerable_Loop()
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

    [Fact]
    public async Task TraverseSequence()
    {
        var xdoc = XDocument.Parse(Resource.ReadAllText("test_sequence.xml"));
        
        DateHelper.NowOverride = new(2023, 8, 24, 6, 50);
        var testDate = DateHelper.NowCoptic();
        
        LoadContext context = new();
        var sequence = SequenceReader.ParseSequenceXml(xdoc.Root!, context);

        var nodes = await sequence.EnumerateNodes().ToListAsync();
        Assert.Equal(3, nodes.Count);

        // Assert that every node has a unique ID
        var nodeIds = nodes.Select(n => n.Id).ToArray();
        Assert.Equal(nodeIds.ToHashSet().Count, nodeIds.Length);

        var morningDoxologyNode = nodes[0];
        Assert.Equal(0, morningDoxologyNode.Id);

        var adamTheotokiaConclusion = nodes[1];
        Assert.Equal(1, adamTheotokiaConclusion.Id);

        var theotokia = nodes[2];
        Assert.Equal((int)testDate.DayOfWeek + 2, theotokia.Id);

        var theotokiaKeyEx = testDate.DayOfWeek switch
        {
            IsoDayOfWeek.Thursday => "urn:tasbehaorg-cr:471",
            IsoDayOfWeek.Friday => "urn:tasbehaorg-cr:146",
            _ => throw new ArgumentOutOfRangeException()
        };
        Assert.Equal(theotokiaKeyEx, theotokia.DocumentKey);
    }
}