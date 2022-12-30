using CoptLib.IO;
using CoptLib.Models.Text;
using CoptLib.Scripting;
using CoptLib.Writing;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    /// <summary>
    /// A base class for anything that can be placed inside the content of a <see cref="Translation"/>.
    /// </summary>
    public abstract class ContentPart : Definition, IMultilingual
    {
        public ContentPart(IDefinition parent)
        {
            Parent = parent;
            DocContext = parent?.DocContext;

            if (parent is IMultilingual multiParent)
            {
                Language = multiParent.Language;
                Font = multiParent.Font;
            }
        }

        [XmlAttribute]
        public LanguageInfo Language { get; set; }

        [XmlAttribute]
        public string Font { get; set; }

        [XmlIgnore]
        public bool FontHandled { get; protected set; }

        public abstract void HandleFont();

        /// <summary>
        /// Returns the number of rows this part requires to display
        /// all its content, including section headers and stanzas
        /// </summary>
        public virtual int CountRows() => 1;
    }

    public class Stanza : ContentPart, IContent
    {
        private string _sourceText;

        public Stanza(IDefinition parent) : base(parent)
        {

        }

        public string SourceText
        {
            get => _sourceText;
            set
            {
                if (_sourceText != value)
                {
                    FontHandled = false;
                    CommandsHandled = false;
                    Inlines = new()
                    {
                        new Run(value, this)
                    };
                }

                _sourceText = value;
            }
        }

        public bool CommandsHandled { get; set; }

        public InlineCollection Inlines { get; set; }

        public List<TextCommandBase> Commands { get; set; }

        public void HandleCommands() => ContentHelper.HandleCommands(this);

        public override void HandleFont()
        {
            ContentHelper.HandleFont(Inlines);
            FontHandled = true;
        }

        public string GetText() => ContentHelper.GetText(this);
    }

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
