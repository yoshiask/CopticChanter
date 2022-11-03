using CoptLib.IO;
using CoptLib.Scripting;
using CoptLib.Writing;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    /// <summary>
    /// A base class for anything that can be placed inside the content of a <see cref="Translation"/>.
    /// </summary>
    public abstract class ContentPart : Definition, IMultilingual
    {
        public ContentPart(Definition parent)
        {
            Parent = parent;
        }

        [XmlAttribute]
        public Language Language { get; set; }

        [XmlAttribute]
        public string Font { get; set; }

        [XmlIgnore]
        public bool Handled { get; protected set; }

        public abstract void HandleFont();
    }

    public class Stanza : ContentPart, IContent
    {
        private string _sourceText;

        public Stanza(Definition parent) : base(parent)
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

            Commands = Scripting.Scripting.ParseTextCommands(SourceText, DocContext, out var text);
            Text = text;
            HasBeenParsed = true;
        }

        public override string ToString() => SourceText;
    }

    public class Section : ContentPart, IContentCollectionContainer
    {
        public Section(Definition parent) : base(parent)
        {

        }

        [XmlArray]
        public List<ContentPart> Content { get; set; } = new List<ContentPart>();

        [XmlAttribute]
        public string Title { get; set; }
        public string Source { get; set; }

        public int CountRows()
        {
            int count = 0;
            foreach (ContentPart part in Content)
            {
                if (part is Stanza)
                    count++;
                else if (part is Section section)
                    count += section.CountRows() + 1;
            }
            return count;
        }

        public void ParseCommands() => DocReader.RecursiveParseCommands(Content);

        public override void HandleFont()
        {
            if (Handled)
                return;

            foreach (ContentPart part in Content)
                part.HandleFont();
        }
    }
}
