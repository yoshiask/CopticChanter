using System;
using System.Buffers;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Scripting;
using CoptLib.Writing;
using MessagePack;
using OwlCore.Extensions;

namespace CoptLib.Hyperspeed.IO;

public static class HyperspeedDocReader
{
    public static IDefinition DeserializeDefinition(Stream stream)
    {
        var bytes = stream.ToBytes();
        MessagePackReader msg = new(bytes);
        return DeserializeDefinition(msg);
    }
    
    public static IDefinition DeserializeDefinition(MessagePackReader msg)
    {
        var defCode = (HyperspeedDefinitionCode)msg.ReadUInt16();
        var key = msg.ReadString();
        
        IDefinition def = defCode switch
        {
            HyperspeedDefinitionCode.Doc => new Doc(),
            HyperspeedDefinitionCode.Stanza => new Stanza(null),
            HyperspeedDefinitionCode.Section => new Section(null),
            HyperspeedDefinitionCode.Run => new Run(null),
            HyperspeedDefinitionCode.Comment => new Comment(null),
            HyperspeedDefinitionCode.SimpleContent => new SimpleContent(null, null),
            HyperspeedDefinitionCode.Script => ReadScript(msg),
            HyperspeedDefinitionCode.Variable => new Variable(),
            HyperspeedDefinitionCode.Translations => new TranslationCollection(),
            
            _ => throw new ArgumentOutOfRangeException()
        };

        def.Key = key;
        
        if (def is Doc doc)
        {
            doc.Name = msg.ReadEncodedString()!;

            if (msg.ReadBoolean())
            {
                doc.Author = new()
                {
                    FullName = msg.ReadEncodedString(),
                    PhoneNumber = msg.ReadString(),
                    Email = msg.ReadString(),
                    Website = msg.ReadString(),
                };
            }
            
            var translationCount = msg.ReadArrayHeader();
            for (int t = 0; t < translationCount; t++)
            {
                var translation = (ContentPart)DeserializeDefinition(msg);
                doc.Translations.Children.Add(translation);
            }
            
            var definitionCount = msg.ReadArrayHeader();
            for (int d = 0; d < definitionCount; d++)
            {
                doc.AddDefinition(DeserializeDefinition(msg));
            }
        }
        if (def is IMultilingual multilingual)
        {
            var language = msg.ReadString();
            if (language is not null)
                multilingual.Language = LanguageInfo.Parse(language);

            multilingual.Font = msg.ReadString();
        }
        if (def is IContent content)
        {
            // TODO: Optimize inlines
            content.SourceText = msg.ReadEncodedString()!;
        }
        if (def is IContentCollectionContainer contentCollection)
        {
            var sectionSource = msg.ReadEncodedString();
            if (sectionSource is not null)
                contentCollection.Source = new SimpleContent(sectionSource, contentCollection);
            
            var contentCollectionCount = msg.ReadArrayHeader();
            for (int p = 0; p < contentCollectionCount; p++)
            {
                var part = (ContentPart)DeserializeDefinition(msg);
                contentCollection.Children.Add(part);
            }
        }

        // Serialize class-specific properties
        switch (def)
        {
            case Section section:
                var sectionTitle = msg.ReadEncodedString();
                if (sectionTitle is not null)
                    section.SetTitle(sectionTitle);
                break;

            case Variable variable:
                variable.Label = msg.ReadString();
                variable.DefaultValue = msg.ReadEncodedString();
                variable.Configurable = msg.ReadBoolean();
                break;
        }

        return def;
    }

    private static IDefinition ReadScript(this MessagePackReader msg)
    {
        var typeId = msg.ReadString()!;
        var scriptBody = msg.ReadEncodedString()!;
        return (IDefinition)ScriptingEngine.CreateScript(typeId, scriptBody);
    }
    
    private static string? ReadEncodedString(this MessagePackReader msg)
    {
        if (msg.TryReadNil())
            return null;

        var encoding = Encoding.GetEncoding(msg.ReadInt32());
        var bytes = msg.ReadBytes()!.Value.ToArray();
        return encoding.GetString(bytes);
    }
}