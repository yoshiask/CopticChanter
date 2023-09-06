using CoptLib.Scripting;
using CoptLib.Writing;
using System.Collections.Generic;

namespace CoptLib.Models;

/// <summary>
/// A base class for anything that can be placed inside the content of a <see cref="IContentCollectionContainer"/>.
/// </summary>
public abstract class ContentPart : Definition, IMultilingual, ISupportsTextCommands
{
    public ContentPart(IDefinition? parent)
    {
        Parent = parent;
        DocContext = parent?.DocContext;
        Commands = new List<TextCommandBase>();

        if (parent is IMultilingual multiParent)
        {
            Language = multiParent.Language;
            Font = multiParent.Font;
        }
        else
        {
            Language = LanguageInfo.Default;
        }
    }

    public LanguageInfo Language { get; set; }

    public string? Font { get; set; }

    public RoleInfo? Role { get; set; }

    public bool FontHandled { get; protected set; }

    public bool CommandsHandled { get; set; }

    public List<TextCommandBase> Commands { get; set; }

    /// <summary>
    /// Returns the number of rows this part requires to display
    /// all its content, including section headers and stanzas.
    /// </summary>
    public virtual int CountRows()
    {
        int count = 1;

        if (Role is not null)
            ++count;

        return count;
    }

    /// <summary>
    /// Enumerates all content, flattening the document tree.
    /// </summary>
    public virtual IEnumerable<IDefinition> Flatten()
    {
        if (Role is not null)
            yield return Role;

        yield return this;
    }

    public abstract void HandleFont();

    public abstract void HandleCommands();
}