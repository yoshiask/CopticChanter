﻿using CoptLib.Scripting;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    /// <summary>
    /// A base class for anything that can be placed inside the content of a <see cref="Translation"/>.
    /// </summary>
    public abstract class ContentPart : Definition
    {
        public ContentPart(Translation parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// A key that can be used to identify this specific content part.
        /// This value does not have to be unique between documents or translations.
        /// </summary>
        /// <remarks>
        /// If the same key is used on content parts from two different translations
        /// in the same document, any scripts that use that key will reference
        /// both parts.
        /// </remarks>
        [XmlAttribute]
        public string Key { get; set; }

        [XmlAttribute]
        public Language Language { get; set; }

        [XmlAttribute]
        public string Font { get; set; }

        [XmlIgnore]
        public Translation Parent { get; set; }
    }

    public class Stanza : ContentPart, IContent
    {
        private string _sourceText;

        public Stanza(Translation parent) : base(parent)
        {

        }

        [XmlText]
        public string SourceText
        {
            get => _sourceText;
            set
            {
                if (_sourceText != value)
                    HasBeenParsed = false;
                _sourceText = value;
            }
        }

        public bool HasBeenParsed { get; private set; }

        public string Text { get; private set; }

        public List<TextCommandBase> Commands { get; private set; }

        public void ParseCommands()
        {
            if (HasBeenParsed)
                return;

            Commands = Scripting.Scripting.ParseTextCommands(SourceText, Parent.Parent, out var text);
            Text = text;
        }

        public override string ToString() => SourceText;
    }

    public class Section : ContentPart, IContentCollectionContainer
    {
        public Section(Translation parent) : base(parent)
        {

        }

        [XmlArray]
        public List<ContentPart> Content { get; set; } = new List<ContentPart>();

        [XmlAttribute]
        public string Title { get; set; }

        /// <summary>
        /// Returns the number of rows this section requires to display
        /// all section headers and stanzas
        /// </summary>
        /// <returns></returns>
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
    }
}