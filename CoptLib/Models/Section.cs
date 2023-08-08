using System.Collections.Generic;
using System.Linq;
using CoptLib.Writing;

namespace CoptLib.Models;

public class Section : ContentPart, IContentCollectionContainer
{
    public Section(IDefinition? parent) : base(parent)
    {
    }

    public IContent? Title { get; private set; }

    public SimpleContent? Source { get; set; }

    public List<ContentPart> Children { get; } = new();

    public void SetTitle(IContent? title)
    {
        if (title is IMultilingual titleMulti)
        {
            titleMulti.Font ??= Font;
            if (titleMulti.Language == LanguageInfo.Default)
                titleMulti.Language = Language;
        }
            
        if (title is not null)
            title.Parent = this;
            
        Title = title;
    }
        
    public void SetTitle(string title)
    {
        Title = new Stanza(this)
        {
            SourceText = title,
            Font = Font,
            Language = Language,
        };
    }

    public override int CountRows()
    {
        int count = Children.Sum(p => p.CountRows());

        if (Title != null)
            count++;

        return count;
    }

    public override void HandleCommands()
    {
        if (CommandsHandled)
            return;
            
        Doc.RecursiveTransform(Children);
        Title?.HandleCommands();
            
        CommandsHandled = true;
    }

    public override void HandleFont()
    {
        if (FontHandled)
            return;

        foreach (ContentPart part in Children)
            part.HandleFont();

        if (Title is IMultilingual multiTitle)
            multiTitle.HandleFont();
            
        FontHandled = true;
    }
}