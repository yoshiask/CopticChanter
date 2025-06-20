using System;
using System.Collections.Generic;
using CoptLib.IO;

namespace CoptLib.Trees.Binary;

public class BinaryNode<T>
{
    public BinaryNode<T>? Parent { get; set; }

    public BinaryNode<T>? Left { get; set; }

    public BinaryNode<T>? Right { get; set; }

    public T? Value { get; set; }

    public bool IsLeaf => Left is null && Right is null;

    public bool IsRoot => Parent is null;

    public BinaryNode(T? value = default, BinaryNode<T>? left = null, BinaryNode<T>? right = null, BinaryNode<T>? parent = null)
    {
        Parent = parent;
        Value = value;

        if (left is not null)
        {
            left.Parent = this;
            Left = left;
        }

        if (right is not null)
        {
            right.Parent = this;
            Right = right;
        }
    }

    /// <summary>
    /// Traverses the provided path by moving left and right as instructed by the bitstream.
    /// </summary>
    /// <param name="path">
    /// The path to follow, where <see langword="true"/> is interpreted as right, and <see langword="false"/> as left.
    /// </param>
    /// <returns>The descendant node once the bitstream is exhausted.</returns>
    /// <exception cref="IndexOutOfRangeException">The bitstream was longer than the depth of the tree.</exception>
    public BinaryNode<T> Traverse(ISimpleBitstream path)
    {
        BinaryNode<T>? node = this;
        foreach (var takeRight in path)
        {
            node = takeRight ? node?.Right : node?.Left;

            if (node is null)
                throw new IndexOutOfRangeException();
        }

        return node;
    }

    public IEnumerable<BinaryNode<T>> EnumeratePreOrder()
    {
        yield return this;

        if (Left is not null)
            foreach (var leftDescendantNode in Left.EnumeratePostOrder())
                yield return leftDescendantNode;
            
        if (Right is not null)
            foreach (var rightDescendantNode in Right.EnumeratePostOrder())
                yield return rightDescendantNode;
    }

    public IEnumerable<BinaryNode<T>> EnumerateInOrder()
    {
        if (Left is not null)
            foreach (var leftDescendantNode in Left.EnumerateInOrder())
                yield return leftDescendantNode;

        yield return this;
            
        if (Right is not null)
            foreach (var rightDescendantNode in Right.EnumerateInOrder())
                yield return rightDescendantNode;
    }

    public IEnumerable<BinaryNode<T>> EnumeratePostOrder()
    {
        if (Left is not null)
            foreach (var leftDescendantNode in Left.EnumeratePostOrder())
                yield return leftDescendantNode;

        if (Right is not null)
            foreach (var rightDescendantNode in Right.EnumeratePostOrder())
                yield return rightDescendantNode;

        yield return this;
    }

    public IEnumerable<BinaryNode<T>> EnumerateLevelOrder()
    {
        Queue<BinaryNode<T>> queue = new();
        queue.Enqueue(this);

        while (queue.Count > 0)
        {
            var node = queue.Dequeue();
            yield return node;

            if (node.Left != null)
                queue.Enqueue(node.Left);
            
            if (node.Right != null)
                queue.Enqueue(node.Right);
        }
    }

    /// <summary>
    /// Swaps the left and right child nodes.
    /// </summary>
    public void SwapChildren()
    {
        var nodeA = Left;
        var nodeB = Right;
        Left = nodeB;
        Right = nodeA;
    }

    public BinaryNode<T> GetRoot()
    {
        var currentNode = this;
        while (currentNode.Parent is not null)
            currentNode = currentNode.Parent;
        return currentNode;
    }
    
    public virtual string GetLabel() => Value?.ToString() ?? string.Empty;
}
