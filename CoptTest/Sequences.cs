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
    public async Task ReadAndTraverseSequence()
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
        Assert.IsType<ConstantSequenceNode>(morningDoxologyNode);

        var adamTheotokiaConclusionNode = nodes[1];
        Assert.Equal(1, adamTheotokiaConclusionNode.Id);
        Assert.IsType<ScriptedSequenceNode>(adamTheotokiaConclusionNode);

        var theotokiaNode = nodes[2];
        Assert.Equal((int)testDate.DayOfWeek + 2, theotokiaNode.Id);
        Assert.IsType<EndSequenceNode>(theotokiaNode);
        Assert.Null(theotokiaNode.NextNode(context));

        var theotokiaKeyEx = testDate.DayOfWeek switch
        {
            IsoDayOfWeek.Thursday => "urn:tasbehaorg-cr:471",
            IsoDayOfWeek.Friday => "urn:tasbehaorg-cr:146",
            _ => throw new ArgumentOutOfRangeException()
        };
        Assert.Equal(theotokiaKeyEx, theotokiaNode.DocumentKey);
    }
    
    [Theory]
    [InlineData("test_sequence.xml")]
    public void SequenceWriter_OpenAndWriteNoChanges(string file)
    {
        LoadContext context = new();
        
        var xmlEx = Resource.ReadAllText(file);
        var sequenceEx = SequenceReader.ParseSequenceXml(XDocument.Parse(xmlEx), context);

        SequenceWriter writer = new(sequenceEx);
        var xmlAc = writer.SerializeToXml().ToString();
        var sequenceAc = SequenceReader.ParseSequenceXml(XDocument.Parse(xmlAc), context);

        Assert.Equal(sequenceEx.Name, sequenceAc.Name);
        Assert.Equal(sequenceEx.Key, sequenceAc.Key);
        Assert.Equal(sequenceEx.RootNodeId, sequenceAc.RootNodeId);
        Assert.Equal(sequenceEx.Nodes.Count, sequenceAc.Nodes.Count);

        foreach (var nodeEx in sequenceEx.Nodes.Values)
        {
            var nodeAc = sequenceAc.Nodes[nodeEx.Id];
            
            // DO NOT use simple equality: complex nodes
            // (e.g. Scripted) will always fail.
            Assert.Equal(nodeEx.Id, nodeAc.Id);
            Assert.Equal(nodeEx.DocumentKey, nodeAc.DocumentKey);
            Assert.Equal(nodeEx.GetType(), nodeAc.GetType());
        }
    }
}