using CoptLib.Writing;
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

        [XmlAttribute]
        public RoleInfo Role { get; set; }

        [XmlIgnore]
        public bool FontHandled { get; protected set; }

        public abstract void HandleFont();

        /// <summary>
        /// Returns the number of rows this part requires to display
        /// all its content, including section headers and stanzas
        /// </summary>
        public virtual int CountRows() => 1;
    }
}
