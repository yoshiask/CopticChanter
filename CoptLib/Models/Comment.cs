namespace CoptLib.Models;

public class Comment : Paragraph
{
    public Comment(IDefinition? parent) : this(CommentType.Other, parent)
    {
    }

    public Comment(CommentType type, IDefinition? parent) : base(parent)
    {
        Type = type;
    }

    /// <summary>
    /// The type of comment.
    /// </summary>
    public CommentType Type { get; set; }
}

public enum CommentType
{
    Other,
    Tune,
    Description,
    Explanation,
    Commentary,
    Role,
}