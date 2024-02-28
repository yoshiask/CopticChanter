using System;
using System.IO;
using System.Xml.Linq;
using CommunityToolkit.Diagnostics;
using CoptLib.Extensions;
using CoptLib.Models.Sequences;
using CoptLib.Scripting;

namespace CoptLib.IO;

public static class SequenceReader
{
    /// <inheritdoc cref="ParseSequenceXml(System.Xml.Linq.XElement,CoptLib.IO.ILoadContext)"/>/>
    public static Sequence ParseSequenceXml(XDocument xdoc, ILoadContext context)
    {
        Guard.IsNotNull(xdoc.Root);
        return ParseSequenceXml(xdoc.Root, context);
    }
    
    public static Sequence ParseSequenceXml(XElement rootElem, ILoadContext context)
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

        var nodeType = EnumExtensions.Parse<SequenceNodeType>(elem.Name.LocalName.ToLowerInvariant(), true);
        switch (nodeType)
        {
            case SequenceNodeType.End:
                return new EndSequenceNode(id, documentKey);
            case SequenceNodeType.Const:
                return new ConstantSequenceNode(id, documentKey, ParseConstantNodeId(elem.Value));
            case SequenceNodeType.Script:
                var scriptTypeId = elem.Attribute("TypeId")?.Value ?? "csharp";
                
                var genericScript = ScriptingEngine.CreateScript(scriptTypeId, elem.Value);
                Guard.IsNotNull(genericScript);
                
                return new ScriptedSequenceNode(id, documentKey, genericScript);
            default:
                throw new ArgumentOutOfRangeException(string.Empty, $"Unknown sequence node type '{nodeType}'");
        }
    }

    private static int? ParseConstantNodeId(string value) => value is "" or "{end}" ? null : int.Parse(value);
}

internal enum SequenceNodeType : byte
{
    End, Const, Script,
}