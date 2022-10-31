using CoptLib.Writing;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    /// <summary>
    /// A base class for anything that can be defined in a <see cref="Doc"/>.
    /// </summary>
    public abstract class Definition
    {
        /// <summary>
        /// A key that can be used to identify this object in scripts.
        /// This value must be unique within the document.
        /// </summary>
        [XmlAttribute]
        public string Key { get; set; }

        [XmlIgnore]
        public Doc DocContext { get; set; }
    }

    public class Script : Definition
    {
        [XmlText]
        public string LuaScript { get; set; }
    }

    public class Variable : Definition
    {
        [XmlAttribute]
        public bool Configurable { get; set; }

        [XmlAttribute]
        public string Label { get; set; }

        [XmlAttribute]
        public object DefaultValue { get; set; }
    }

    public class String : Definition
    {
        [XmlText]
        public string Value { get; set; }

        [XmlAttribute]
        public Language Language { get; set; }

        [XmlAttribute]
        public string Font { get; set; }
    }
}
