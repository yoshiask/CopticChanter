using System.Text;
using CoptLib.Trees.Binary;

namespace CoptLib.Writing.Linguistics.XBar;

/// <summary>
/// A node in an X-bar tree structure.
/// </summary>
public class XBarNode(Tag tag,
    IStructuralElement? value = null,
    XBarNode? left = null, 
    XBarNode? right = null,
    XBarNode? parent = null)
    : BinaryNode<IStructuralElement>(value, left, right, parent)
{
    public Tag Tag { get; } = tag;

    public XBarNode? XBarParent
    {
        get => Parent as XBarNode;
        set => Parent = value;
    }
    
    public XBarNode? XBarLeft
    {
        get => Left as XBarNode;
        set => Left = value;
    }
    
    public XBarNode? XBarRight
    {
        get => Right as XBarNode;
        set => Right = value;
    }

    public XBarNode? Complement => XBarParent?.XBarLeft != this
        ? XBarParent?.XBarLeft
        : XBarParent?.XBarRight;

    public XBarNode? Specifier => XBarParent?.Complement;
    
    public XBarNode GetXBarRoot()
    {
        var currentNode = this;
        while (currentNode.XBarParent is not null)
            currentNode = currentNode.XBarParent;
        return currentNode;
    }

    public XBarNode? GetParentPhrase(bool includeSelf = false)
    {
        var currentNode = includeSelf ? this : XBarParent;
        
        while (currentNode is not null && currentNode.Tag.Type is not XBarNodeType.Phrase)
            currentNode = currentNode.XBarParent;
        
        return currentNode?.Tag.Type is XBarNodeType.Phrase
            ? currentNode
            : null;
    }

    public string ToPhraseStructureString()
    {
        var sb = new StringBuilder();
        sb.Append(Tag);
        sb.Append('>');

        if (IsLeaf)
        {
            sb.Append(Value);
        }
        else
        {
            sb.Append(XBarLeft?.Tag);
            sb.Append(' ');
            sb.Append(XBarRight?.Tag);
        }
        
        return sb.ToString();
    }

    public override string GetLabel() => Value?.ToString() ?? Tag.ToString();
}

public static class XBarNodeExtensions
{
    /// <summary>
    /// Represents the binary tree using a generic syntax tree string.
    /// </summary>
    /// <param name="sb">A <see cref="StringBuilder"/> to write to.</param>
    public static StringBuilder SerializeToSyntaxTree(this XBarNode root, StringBuilder sb)
    {
        sb.Append('[');
        sb.Append(root.Tag);

        if (root.Value is not null)
        {
            sb.Append(' ');
            sb.Append(root.Value);
        }

        if (root.XBarLeft is not null)
        {
            sb.Append(' ');
            root.XBarLeft.SerializeToSyntaxTree(sb);
        }

        if (root.XBarRight is not null)
        {
            sb.Append(' ');
            root.XBarRight.SerializeToSyntaxTree(sb);
        }
        
        sb.Append(']');
        return sb;
    }

    /// <summary>
    /// Represents the binary tree using a generic syntax tree string.
    /// </summary>
    public static string SerializeToSyntaxTree(this XBarNode node)
        => node.SerializeToSyntaxTree(new StringBuilder()).ToString();
}