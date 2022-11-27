using CoptLib.IO;
using CoptLib.Scripting;
using CoptLib.Writing;
using System.Collections;
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
        public KnownLanguage Language { get; set; }

        [XmlAttribute]
        public string Font { get; set; }

        [XmlIgnore]
        public bool Handled { get; protected set; }

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

        [XmlText]
        public string SourceText
        {
            get => _sourceText;
            set
            {
                if (_sourceText != value)
                {
                    HasBeenParsed = false;
                    Handled = false;
                }
                _sourceText = value;
            }
        }

        public bool HasBeenParsed { get; private set; }

        public string Text { get; set; }

        public List<TextCommandBase> Commands { get; private set; }

        public override void HandleFont()
        {
            if (!Handled && CopticFont.TryFindFont(Font, out var font))
            {
                Text = font.Convert(Text);
                Handled = true;
            }
        }

        public void ParseCommands()
        {
            if (HasBeenParsed)
                return;

            Commands = Scripting.Scripting.ParseTextCommands(this, out var text);
            Text = text;
            HasBeenParsed = true;
        }

        public override string ToString() => Text ?? SourceText;
    }

    public class Section : ContentPart, IContentCollectionContainer
    {
        public Section(IDefinition parent) : base(parent)
        {

        }

        public SimpleContent Title { get; set; }

        public SimpleContent Source { get; set; }

        public List<ContentPart> Children { get; } = new();

        public override int CountRows()
        {
            int count = Children.Sum(p => p.CountRows());

            if (Title != null)
                count++;

            return count;
        }

        public void ParseCommands()
        {
            DocReader.RecursiveTransform(Children);
            Title?.ParseCommands();
        }

        public override void HandleFont()
        {
            if (Handled)
                return;

            foreach (ContentPart part in Children)
                part.HandleFont();

            if (Title != null && Title is IMultilingual multiTitle)
                multiTitle.HandleFont();
        }
    }
}
