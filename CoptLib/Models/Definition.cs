using CoptLib.Writing;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.Models
{
    /// <summary>
    /// An interface for anything that can be defined in a <see cref="Doc"/>.
    /// </summary>
    public interface IDefinition
    {
        /// <summary>
        /// A key that can be used to identify this object in scripts.
        /// This value must be unique within the document.
        /// </summary>
        string Key { get; set; }

        Doc DocContext { get; set; }

        IDefinition Parent { get; set; }
    }

    /// <summary>
    /// A base class for anything that can be defined in a <see cref="Doc"/>.
    /// </summary>
    public abstract class Definition : IDefinition
    {
        [XmlAttribute]
        public string Key { get; set; }

        [XmlIgnore]
        public Doc DocContext { get; set; }

        [XmlIgnore]
        public IDefinition Parent { get; set; }
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
}
