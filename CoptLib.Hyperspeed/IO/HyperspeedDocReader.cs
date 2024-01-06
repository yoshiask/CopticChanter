using System;
using System.IO;
using System.Text;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Scripting;
using CoptLib.Writing;

namespace CoptLib.Hyperspeed.IO;

public static class HyperspeedDocReader
{
    public static IDefinition? ReadDefinition(Stream stream, ILoadContext? context = null, IDefinition? parent = null)
    {
        BinaryReader reader = new(stream);
        return ReadObject<IDefinition>(reader, context, parent);
    }

    public static T? ReadObject<T>(this BinaryReader reader, ILoadContext? context = null, IDefinition? parent = null)
    {
        var obj = reader.ReadObject(context, parent);
        return obj is null ? default : (T)obj;
    }
    
    public static object? ReadObject(this BinaryReader reader,
        ILoadContext? context = null, IDefinition? parent = null)
    {
        var defCode = (HyperspeedDefinitionCode)reader.ReadUInt16();
        if (defCode is HyperspeedDefinitionCode.Null)
            return null;
        
        var key = reader.ReadNullableString();
        
        IDefinition def = defCode switch
        {
            HyperspeedDefinitionCode.Doc => new Doc(context),
            HyperspeedDefinitionCode.Stanza => new Stanza(parent),
            HyperspeedDefinitionCode.Section => new Section(parent),
            HyperspeedDefinitionCode.Run => new Run(parent),
            HyperspeedDefinitionCode.Comment => new Comment(parent),
            HyperspeedDefinitionCode.SimpleContent => new SimpleContent(null, parent),
            HyperspeedDefinitionCode.Script => ReadScript(reader),
            HyperspeedDefinitionCode.Variable => new Variable(),
            HyperspeedDefinitionCode.Translations => new TranslationCollection(),
            
            _ => throw new ArgumentOutOfRangeException()
        };

        def.Key = key;
        def.Parent = parent;
        
        if (def is Doc doc)
        {
            doc.Name = reader.ReadEncodedString()!;

            if (reader.ReadBoolean())
            {
                doc.Author = new()
                {
                    FullName = reader.ReadEncodedString(),
                    PhoneNumber = reader.ReadNullableString(),
                    Email = reader.ReadNullableString(),
                    Website = reader.ReadNullableString(),
                };
            }
            
            var translationCount = reader.ReadInt32();
            for (int t = 0; t < translationCount; t++)
            {
                var translation = ReadObject<ContentPart>(reader, context, def)!;
                doc.Translations.Children.Add(translation);
            }
            
            var definitionCount = reader.ReadInt32();
            for (int d = 0; d < definitionCount; d++)
            {
                doc.AddDefinition(ReadObject<IDefinition>(reader, context, def)!);
            }
        }
        if (def is IMultilingual multilingual)
        {
            var language = reader.ReadNullableString();
            if (language is not null)
                multilingual.Language = LanguageInfo.Parse(language);

            multilingual.Font = reader.ReadNullableString();
        }
        if (def is IContent content)
        {
            // TODO: Optimize inlines
            content.SourceText = reader.ReadEncodedString()!;
        }
        if (def is IContentCollectionContainer contentCollection)
        {
            contentCollection.Source = reader.ReadObject<SimpleContent>(context, contentCollection);
            
            var contentCollectionCount = reader.ReadInt32();
            for (int p = 0; p < contentCollectionCount; p++)
            {
                var part = ReadObject<ContentPart>(reader, context, contentCollection)!;
                contentCollection.Children.Add(part);
            }
        }

        // Serialize class-specific properties
        switch (def)
        {
            case Section section:
                var sectionTitle = reader.ReadObject<IContent>(context, section);
                section.SetTitle(sectionTitle);
                break;

            case Run run:
                run.Text = reader.ReadEncodedString()!;
                break;
            
            case TranslationRunCollection runCollection:
                var runCollectionCount = reader.ReadInt32();
                for (int p = 0; p < runCollectionCount; p++)
                {
                    var translationRun = ReadObject<Run>(reader, context, runCollection)!;
                    runCollection.AddRun(translationRun);
                }
                break;
            
            case Variable variable:
                variable.Label = reader.ReadString();
                variable.DefaultValue = reader.ReadObject(context, variable);
                variable.Configurable = reader.ReadBoolean();
                break;
        }

        return def;
    }

    private static IDefinition ReadScript(this BinaryReader reader)
    {
        var typeId = reader.ReadString()!;
        var scriptBody = reader.ReadEncodedString()!;
        return (IDefinition)ScriptingEngine.CreateScript(typeId, scriptBody);
    }
    
    private static string? ReadEncodedString(this BinaryReader reader)
    {
        if (reader.PeekChar() == 0)
        {
            ++reader.BaseStream.Position;
            return null;
        }

        var encoding = Encoding.GetEncoding(reader.ReadInt32());
        var bytes = reader.ReadBytes();
        return encoding.GetString(bytes);
    }

    private static byte[] ReadBytes(this BinaryReader reader)
    {
        var length = reader.ReadInt32();
        return reader.ReadBytes(length);
    }

    private static string? ReadNullableString(this BinaryReader reader)
    {
        if (reader.PeekChar() == 0)
        {
            ++reader.BaseStream.Position;
            return null;
        }

        return reader.ReadString();
    }
}