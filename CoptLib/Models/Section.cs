using CoptLib.IO;
using System.Collections.Generic;
using System.Linq;

namespace CoptLib.Models
{
    public class Section : ContentPart, IContentCollectionContainer
    {
        public Section(IDefinition parent) : base(parent)
        {

        }

        public IContent Title { get; set; }

        public SimpleContent Source { get; set; }

        public List<ContentPart> Children { get; } = new();

        public override int CountRows()
        {
            int count = Children.Sum(p => p.CountRows());

            if (Title != null)
                count++;

            return count;
        }

        public void HandleCommands()
        {
            DocReader.RecursiveTransform(Children);
            Title?.HandleCommands();
        }

        public override void HandleFont()
        {
            if (FontHandled)
                return;

            foreach (ContentPart part in Children)
                part.HandleFont();

            if (Title != null && Title is IMultilingual multiTitle)
                multiTitle.HandleFont();
        }
    }
}
