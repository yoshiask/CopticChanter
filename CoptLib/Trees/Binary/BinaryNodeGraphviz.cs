using System.Linq;
using System.Text;

namespace CoptLib.Trees.Binary;

public static class BinaryNodeGraphviz
{
    /// <summary>
    /// Represents the binary tree using Graphviz's DOT language.
    /// </summary>
    /// <param name="sb">A <see cref="StringBuilder"/> to write to.</param>
    public static StringBuilder SerializeToDot<T>(this BinaryNode<T> root, StringBuilder sb)
    {
        sb.AppendLine("strict graph {");
        
        // Set appropriate layout for a binary tree
        sb.AppendLine("    rankdir=\"BT\"");
        sb.AppendLine();
        
        // Adjust styling to look a bit cleaner
        sb.AppendLine("    splines=line");
        sb.AppendLine("    node [shape=none]");
        sb.AppendLine("    edge [headport=s tailport=n]");
        sb.AppendLine();

        var nodes = root.EnumerateLevelOrder().ToList();

        sb.AppendLine($"    n{nodes[0].GetHashCode()} [label=\"{nodes[0].GetLabel()}\"]");

        foreach (var node in nodes)
        {
            if (node.Parent is null)
                continue;

            var id = node.GetHashCode();
            sb.AppendLine($"    n{id} [label=\"{node.GetLabel()}\"]");
            sb.AppendLine($"    n{id} -- n{node.Parent.GetHashCode()}");
        }

        sb.AppendLine("}");

        return sb;
    }

    /// <summary>
    /// Represents the binary tree using Graphviz's DOT language.
    /// </summary>
    public static string SerializeToDot<T>(this BinaryNode<T> node)
        => node.SerializeToDot(new StringBuilder()).ToString();
}