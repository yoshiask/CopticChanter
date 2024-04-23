using CoptLib.Extensions;

namespace CoptLib.Models;

public class Comment(CommentType type, IDefinition? parent) : Paragraph(parent)
{
    public Comment(IDefinition? parent) : this(CommentType.Other, parent)
    {
    }

    /// <summary>
    /// The type of comment.
    /// </summary>
    public CommentType Type { get; set; } = type;

    public override void HandleCommands()
    {
        if (!CommandsHandled
            && Type == CommentType.Role
            && (DocContext?.Context.TryLookupDefinition(SourceText, out var roleDef) ?? false)
            && roleDef is RoleInfo role)
        {
            var roleRun = role.GetByLanguage(this.GetLanguage());
            Inlines = [roleRun];
            role.References.Add(this);
        }
        
        base.HandleCommands();
    }
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