using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CoptLib.XML
{
    [XmlRoot(ElementName = "CopticFont")]
    public class FontXml
    {
        [XmlElement]
        public string Name;

        [XmlElement]
        public string FontName;

        [XmlElement]
        public bool IsJenkimBefore = true;

        [XmlElement]
        public bool IsCopticStandard = false;

        [XmlArray]
        [XmlArrayItem(ElementName = "Char")]
        public List<MapXml> Charmap;

        public static FontXml ToFontXml(CopticFont font)
        {
            var xml = new FontXml()
            {
                Name = font.Name,
                FontName = font.FontName,
                IsJenkimBefore = font.IsJenkimBefore,
                IsCopticStandard = font.IsCopticStandard,
                Charmap = new List<MapXml>()
            };
            foreach (string key in font.Charmap.Keys)
            {
                string value;
                font.Charmap.TryGetValue(key, out value);
                var pair = new MapXml()
                {
                    BaseCharacter = key,
                    NewCharacter = value
                };
                xml.Charmap.Add(pair);
            }
            return xml;
        }
        public CopticFont ToCopticFont()
        {
            var font = new CopticFont()
            {
                Name = Name,
                FontName = FontName,
                IsJenkimBefore = IsJenkimBefore,
                IsCopticStandard = IsCopticStandard,
                Charmap = new Dictionary<string, string>()
            };
            foreach (MapXml map in Charmap)
            {
                font.Charmap.Add(map.BaseCharacter, map.NewCharacter);
            }
            return font;
        }

        public class MapXml
        {
            [XmlAttribute("Base")]
            public string BaseCharacter;

            [XmlAttribute("New")]
            public string NewCharacter;
        }
    }
}
