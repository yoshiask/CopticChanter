using System;
using System.IO;
using System.Xml.Linq;
using CoptLib.Models.Sequences;
using CoptLib.Scripting;
using CoptLib.Scripting.Typed;

namespace CoptLib.IO;

public static class SequenceReader
{
    public static Sequence ParseSequenceXml(XElement rootElem, LoadContextBase context)
    {
        var rootNodeIdStr = rootElem.Element(nameof(Sequence.RootNodeId))?.Value;
        if (!int.TryParse(rootNodeIdStr, out var rootNodeId))
            throw new InvalidDataException($"'{rootNodeIdStr}' is not a valid node ID.");

        var key = rootElem.Element(nameof(Sequence.Key))?.Value;
        var name = rootElem.Element(nameof(Sequence.Name))?.Value;

        Sequence sequence = new(rootNodeId, context, key)
        {
            Name = name
        };

        var sequenceNodesElem = rootElem.Element(nameof(Sequence.Nodes));
        if (sequenceNodesElem is null)
            throw new InvalidDataException("A sequence must define its nodes.");
        
        foreach (var sequenceNodeElem in sequenceNodesElem.Elements())
            sequence.Nodes.Add(ParseSequenceNodeXml(sequenceNodeElem));

        return sequence;
    }
    
    public static SequenceNode ParseSequenceNodeXml(XElement elem)
    {
        var idStr = elem.Attribute(nameof(SequenceNode.Id))?.Value;
        if (!int.TryParse(idStr, out var id))
            throw new InvalidDataException($"'{idStr}' is not a valid sequence node ID.");

        var documentKey = elem.Attribute(nameof(SequenceNode.DocumentKey))?.Value;
        if (documentKey is null)
            throw new InvalidDataException("A document key must be specified.");

        var nodeType = elem.Name.LocalName.ToLowerInvariant();
        return nodeType switch
        {
            "end" => new DelegateSequenceNode(id, documentKey, (_, _) => null),
            "script" => CreateScriptedSequenceNode(id, documentKey, elem.Value),
            "const" => new DelegateSequenceNode(id, documentKey, (_, _) => ParseConstantNodeId(elem.Value)),
            _ => throw new ArgumentOutOfRangeException(string.Empty, $"Unknown sequence node type '{nodeType}'")
        };
    }

    private static ScriptedSequenceNode CreateScriptedSequenceNode(int id, string documentKey, string scriptBody)
    {
        DotNetScript<NullableIntScriptImplementation, int?> script = new(scriptBody);
        return new ScriptedSequenceNode(id, documentKey, script);
    }

    private static int? ParseConstantNodeId(string value) => value is "" or "{end}" ? null : int.Parse(value);
}