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
        public Language Language { get; set; }

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

        public string Text { get; private set; }

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

            Commands = Scripting.Scripting.ParseTextCommands(this, DocContext, out var text);
            Text = text;
            HasBeenParsed = true;
        }

        public override string ToString() => SourceText;
    }

    public class Section : ContentPart, IContentCollectionContainer
    {
        protected List<ContentPart> _content = new();

        public Section(IDefinition parent) : base(parent)
        {

        }

        public IContent Title { get; set; }

        public string Source { get; set; }

        public int Count => _content.Count;

        public bool IsReadOnly => false;

        public override int CountRows()
        {
            int count = _content.Sum(p => p.CountRows());

            if (Title != null)
            {
                if (Title is ContentPart partTitle)
                    count += partTitle.CountRows();
                else
                    count++;
            }

            return count;
        }

        public void ParseCommands()
        {
            DocReader.RecursiveParseCommands(_content);
            Title?.ParseCommands();
        }

        public override void HandleFont()
        {
            if (Handled)
                return;

            foreach (ContentPart part in _content)
                part.HandleFont();

            if (Title != null && Title is IMultilingual multiTitle)
                multiTitle.HandleFont();
        }

        public IEnumerator<ContentPart> GetEnumerator() => _content.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(ContentPart item) => _content.Add(item);

        public void Clear() => _content.Clear();

        public bool Contains(ContentPart item) => _content.Contains(item);

        public void CopyTo(ContentPart[] array, int arrayIndex) => _content.CopyTo(array, arrayIndex);

        public bool Remove(ContentPart item) => _content.Remove(item);
    }
}
