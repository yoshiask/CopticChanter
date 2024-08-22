using System;
using System.Runtime.CompilerServices;

namespace CoptLib.Writing.Linguistics.XBar;

public class XPhrase : BinaryNode<IStructuralElement>
{
    public XPhrase(BinaryNode<IStructuralElement> head,
        BinaryNode<IStructuralElement>? complement = null,
        BinaryNode<IStructuralElement>? specifier = null)
    {
        PhrasalCategory? category = null;

        //Bar = new(category, )
    }

    public BinaryNode<IStructuralElement>? Specifier
    {
        get => Left;
        set => Left = value;
    }

    public BinaryNode<IStructuralElement> Bar
    {
        get => Right ?? throw ThrowMissingNodeException();
        set => Right = value ?? throw ThrowMissingNodeException();
    }

    public BinaryNode<IStructuralElement> Head
    {
        get => Bar.Left ?? throw ThrowMissingNodeException();
        set => Bar.Left = value ?? throw ThrowMissingNodeException();
    }

    public BinaryNode<IStructuralElement>? Complement
    {
        get => Bar.Right;
        set => Bar.Right = value;
    }

    private static Exception ThrowMissingNodeException([CallerMemberName] string nodeType = "")
        => new InvalidOperationException($"A {nodeType} node must be present.");
}
