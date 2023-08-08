using CoptLib.Writing;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoptLib.Models;

[XmlRoot(ElementName = "CopticFont")]
public class Font
{
    [XmlElement]
    public string? Name { get; set; }

    [XmlElement]
    public string? FontName { get; set; }

    [XmlElement]
    public bool IsJenkimBefore  { get; set; }

    [XmlArray]
    [XmlArrayItem(ElementName = "Char")]
    public List<MapXml>? Charmap { get; set; }

    public static Font ToFontXml(DisplayFont font)
    {
        var xml = new Font
        {
            Name = font.DisplayName,
            FontName = font.FontFamily,
            IsJenkimBefore = font.IsJenkimBefore,
            Charmap = new List<MapXml>()
        };
        foreach (var (knownCh, ch) in font.Charmap)
        {
            var pair = new MapXml
            {
                BaseCharacter = knownCh,
                NewCharacter = ch
            };
            xml.Charmap.Add(pair);
        }
        return xml;
    }
    public DisplayFont ToDisplayFont()
    {
        var font = new DisplayFont(Name!, FontName!, Name!,
            new(Charmap!.Count), IsJenkimBefore);

        foreach (MapXml map in Charmap)
            font.Charmap.Add(map.BaseCharacter, map.NewCharacter);
            
        return font;
    }

    public class MapXml
    {
        [XmlAttribute("Base")]
        public KnownCharacter BaseCharacter { get; set; }

        [XmlAttribute("New")]
        public char NewCharacter { get; set; }
    }
}