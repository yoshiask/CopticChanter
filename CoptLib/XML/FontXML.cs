using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CoptLib.XML
{
    [XmlRoot(ElementName = "CopticFont")]
    public class FontXML
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
        public List<MapXML> Charmap;

        public static FontXML ToFontXML(CopticFont font)
        {
            var XML = new FontXML()
            {
                Name = font.Name,
                FontName = font.FontName,
                IsJenkimBefore = font.IsJenkimBefore,
                IsCopticStandard = font.IsCopticStandard,
                Charmap = new List<MapXML>()
            };
            foreach (string key in font.Charmap.Keys)
            {
                string value;
                font.Charmap.TryGetValue(key, out value);
                var pair = new MapXML()
                {
                    BaseCharacter = key,
                    NewCharacter = value
                };
                XML.Charmap.Add(pair);
            }
            return XML;
        }
        public CopticFont ToCopticFont()
        {
            var Font = new CopticFont()
            {
                Name = Name,
                FontName = FontName,
                IsJenkimBefore = IsJenkimBefore,
                IsCopticStandard = IsCopticStandard,
                Charmap = new Dictionary<string, string>()
            };
            foreach (MapXML map in Charmap)
            {
                Font.Charmap.Add(map.BaseCharacter, map.NewCharacter);
            }
            return Font;
        }

        public class MapXML
        {
            [XmlAttribute("Base")]
            public string BaseCharacter;

            [XmlAttribute("New")]
            public string NewCharacter;
        }
    }
}
