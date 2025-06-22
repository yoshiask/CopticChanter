using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoptLib.Trees.Binary;

public static class BinaryNodeGraphviz
{
    /// <summary>
    /// Represents the binary tree using Graphviz's DOT language.
    /// </summary>
    public static string SerializeToDot<T>(this BinaryNode<T> root, GraphvizSerializationOptions options)
    {
        StringBuilder sb = new();
        sb.AppendLine("strict graph {");

        if (options.RankDir is not null)
            sb.AppendLine($"    rankdir=\"{options.RankDir}\"");
        
        sb.AppendLine();

        if (options.Splines is not null)
            sb.AppendLine($"    splines=\"{options.Splines}\"");
        
        if (options.NodeShape is not null)
            sb.AppendLine($"    node [shape=\"{options.NodeShape}\"]");
        
        sb.AppendLine($"    edge [headport=\"{options.EdgeHeadPort}\" tailport=\"{options.EdgeTailPort}\"]");
        sb.AppendLine();

        var nodes = root.EnumerateLevelOrder();
        List<BinaryNode<T>>? leafNodes = options.LeafNodeRank is null ? null : [];

        foreach (var node in nodes)
        {
            var id = node.GetHashCode();
            sb.AppendLine($"    n{id} [label=\"{node.GetLabel()}\"]");

            if (node.Parent is not null)
                sb.AppendLine($"    n{id} -- n{node.Parent.GetHashCode()}");
            
            if (node.IsLeaf)
                leafNodes?.Add(node);
        }

        if (leafNodes is not null)
        {
            sb.Append("    { rank=");
            sb.Append(options.LeafNodeRank);
            sb.Append("; ");
            sb.Append(string.Join("; ", leafNodes.Select(n => $"n{n.GetHashCode()}")));
            sb.AppendLine(" }");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <summary>
    /// Represents the binary tree using Graphviz's DOT language with default styling and layout options.
    /// </summary>
    public static string SerializeToDot<T>(this BinaryNode<T> root) =>
        root.SerializeToDot(new GraphvizSerializationOptions());
}

public record GraphvizSerializationOptions(
    string? RankDir = "BT",
    string? Splines = null,
    string? NodeShape = null,
    string EdgeHeadPort = "_",
    string EdgeTailPort = "_",
    string? LeafNodeRank = null)
{
    public static GraphvizSerializationOptions XBar { get; } =
        new("BT", "line", "none", "s", "n", "min");
}