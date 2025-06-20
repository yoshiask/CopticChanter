using System.Linq;
using System.Text;

namespace CoptLib.Writing.Linguistics.XBar;

public static class BinaryNodeExtensions
{
    /// <summary>
    /// Represents the binary tree using Graphviz's DOT language.
    /// </summary>
    /// <param name="sb">A <see cref="StringBuilder"/> to write to.</param>
    public static StringBuilder SerializeToDot<T>(this BinaryNode<T> root, StringBuilder sb)
    {
        sb.AppendLine("strict graph {");
        sb.AppendLine("    rankdir=\"BT\"");
        sb.AppendLine();

        var nodes = root.EnumerateLevelOrder().ToList();

        sb.AppendLine($"    n{nodes[0].GetHashCode()} [label=\"{nodes[0].Value}\"]");

        foreach (var node in nodes)
        {
            if (node.Parent is null)
                continue;

            var id = node.GetHashCode();
            sb.AppendLine($"    n{id} [label=\"{node.Value}\"]");
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
    
    /// <summary>
    /// Represents the binary tree using a generic syntax tree string.
    /// </summary>
    /// <param name="sb">A <see cref="StringBuilder"/> to write to.</param>
    public static StringBuilder SerializeToSyntaxTree<T>(this BinaryNode<T> root, StringBuilder sb)
    {
        sb.Append('[');
        sb.Append(root.Value);

        if (root.Left is not null)
        {
            sb.Append(' ');
            root.Left.SerializeToSyntaxTree(sb);
        }

        if (root.Right is not null)
        {
            sb.Append(' ');
            root.Right.SerializeToSyntaxTree(sb);
        }
        
        sb.Append(']');
        return sb;
    }

    /// <summary>
    /// Represents the binary tree using a generic syntax tree string.
    /// </summary>
    public static string SerializeToSyntaxTree<T>(this BinaryNode<T> node)
        => node.SerializeToSyntaxTree(new StringBuilder()).ToString();
}