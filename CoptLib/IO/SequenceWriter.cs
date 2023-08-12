using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using CoptLib.Models.Sequences;
using CoptLib.Scripting;
using CoptLib.Scripting.Typed;
using OwlCore.Storage;

namespace CoptLib.IO;

public class SequenceWriter
{
    public SequenceWriter(string? key, string? name, int rootNodeId, IEnumerable<SequenceNode> nodes)
    {
        Key = key;
        Name = name;
        RootNodeId = rootNodeId;
        Nodes = nodes.ToList();
    }

    public SequenceWriter(Sequence sequence)
        : this(sequence.Key, sequence.Name, sequence.RootNodeId, sequence.Nodes.Values)
    {
    }

    public SequenceWriter() : this(null, null, 0, new List<SequenceNode>())
    {
    }
    
    public string? Key { get; set; }
    
    public string? Name { get; set; }
    
    public int RootNodeId { get; set; }
    
    public List<SequenceNode> Nodes { get; }

    public async Task WriteXmlAsync(IFile file)
    {
        using var stream = await file.OpenStreamAsync();
        WriteXml(stream);
    }

    public void WriteXml(Stream stream)
    {
        var xdoc = SerializeToXml();

        using StringWriter writer = new();
        using XmlTextWriter xmlWriter = new(stream, Encoding.Unicode);
        xmlWriter.Formatting = Formatting.Indented;
        xmlWriter.Indentation = 4;
        xmlWriter.IndentChar = ' ';

        xdoc.Save(xmlWriter);
    }

    public XDocument SerializeToXml()
    {
        XDocument xdoc = new();
        XElement root = new("Sequence");
        
        root.Add(new XElement(nameof(Key), Key));
        root.Add(new XElement(nameof(Name), Name));
        root.Add(new XElement(nameof(RootNodeId), RootNodeId));
        
        XElement nodesElem = new(nameof(Nodes));
        foreach (var node in Nodes)
            nodesElem.Add(SerializeNodeToXml(node));
        root.Add(nodesElem);

        xdoc.Add(root);
        return xdoc;
    }

    public static XElement SerializeNodeToXml(SequenceNode node)
    {
        XElement elem;

        switch (node)
        {
            case EndSequenceNode:
                elem = new(SequenceNodeType.End.ToString(), SerializeNextNodeResult(null));
                break;
            
            case ConstantSequenceNode constNode:
                elem = new(SequenceNodeType.Const.ToString(), SerializeNextNodeResult(constNode.NextNodeId));
                break;
            
            case ScriptedSequenceNode scriptNode:
                elem = new(SequenceNodeType.Script.ToString());

                string scriptBody = scriptNode.NextDocCommand switch
                {
                    DotNetScript<NullableIntScriptImplementation, int?> dotNetScript => dotNetScript.ScriptBody,
                    _ => throw new Exception(
                        $"'{scriptNode.NextDocCommand.GetType()}' is not supported for script serialization.")
                };

                elem.Add(new XCData(scriptBody));
                break;
            
            default:
                throw new Exception($"Unrecognized {nameof(SequenceNode)} type '{node.GetType()}'");
        }
        
        elem.SetAttributeValue(nameof(node.Id), node.Id);
        elem.SetAttributeValue(nameof(node.DocumentKey), node.DocumentKey);
        
        return elem;
    }

    private static string SerializeNextNodeResult(int? nextNodeId)
        => nextNodeId is null ? "{end}" : nextNodeId.Value.ToString(NumberFormatInfo.InvariantInfo);
}