using System;
using System.IO;
using System.Text;
using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Scripting;
using CoptLib.Writing;

namespace CoptLib.Hyperspeed.IO;

public static class HyperspeedDocWriter
{
    public static void SerializeDefinition(Stream stream, IDefinition def)
    {
        BinaryWriter writer = new(stream);
        SerializeDefinition(writer, def);
    }
    
    public static void SerializeDefinition(BinaryWriter writer, IDefinition def)
    {
        var defCode = def switch
        {
            Doc _                       => HyperspeedDefinitionCode.Doc,
            Stanza _                    => HyperspeedDefinitionCode.Stanza,
            Section _                   => HyperspeedDefinitionCode.Section,
            Run _                       => HyperspeedDefinitionCode.Run,
            Comment _                   => HyperspeedDefinitionCode.Comment,
            SimpleContent _             => HyperspeedDefinitionCode.SimpleContent,
            IScript<object> _           => HyperspeedDefinitionCode.Script,
            Variable _                  => HyperspeedDefinitionCode.Variable,
            TranslationCollection _     => HyperspeedDefinitionCode.Translations,
            
            _ => throw new ArgumentOutOfRangeException(nameof(def))
        };
        
        writer.Write((ushort)defCode);
        writer.WriteNullable(def.Key);

        if (def is Doc doc)
        {
            writer.WriteEncodedString(doc.Name);

            if (doc.Author is null)
            {
                writer.Write(false);
            }
            else
            {
                writer.Write(true);
                writer.WriteEncodedString(doc.Author.FullName);
                writer.WriteNullable(doc.Author.PhoneNumber);
                writer.WriteNullable(doc.Author.Email);
                writer.WriteNullable(doc.Author.Website);
            }

            doc.ApplyTransforms();
            
            writer.Write(doc.Translations.Children.Count);
            foreach (var translation in doc.Translations.Children)
                SerializeDefinition(writer, translation);
            
            // TODO: Optimize direct definitions, only keep scripts and variables
            writer.Write(doc.DirectDefinitions.Count);
            foreach (var directDefinitions in doc.DirectDefinitions)
                SerializeDefinition(writer, directDefinitions);
        }

        if (def is IScript<object> script)
        {
            writer.Write(script.TypeId);
            writer.WriteEncodedString(script.ScriptBody);
        }
        if (def is IMultilingual multilingual)
        {
            var elemLanguage = multilingual.Language;
            var elemFont = multilingual.Font;

            if (def.Parent is not null)
            {
                var parentLanguage = def.Parent.GetLanguage();
                if (!LanguageInfo.IsNullOrDefault(parentLanguage) && parentLanguage == elemLanguage)
                    elemLanguage = null;
                    
                var parentFont = def.Parent.GetFont();
                if (parentFont is not null && parentFont == elemFont)
                    elemFont = null;
            }
            
            multilingual.HandleFont();
            
            writer.WriteNullable(elemLanguage?.ToString());
            writer.WriteNullable(elemFont);
        }
        if (def is IContent content)
        {
            content.HandleCommands();
            
            // TODO: Optimize inlines
            writer.WriteEncodedString(content.GetText());
        }
        if (def is IContentCollectionContainer contentCollection)
        {
            writer.WriteEncodedString(contentCollection.Source?.ToString());
            
            writer.Write(contentCollection.Children.Count);
            foreach (var child in contentCollection.Children)
                SerializeDefinition(writer, child);
        }

        // Serialize class-specific properties
        switch (def)
        {
            case Section section:
                writer.WriteEncodedString(section.Title?.ToString());
                break;

            case Variable variable:
                writer.Write(variable.Label);
                writer.WriteEncodedString(variable.DefaultValue?.ToString());
                writer.Write(variable.Configurable);
                break;
        }
        
        writer.Flush();
    }

    private static void WriteEncodedString(this BinaryWriter writer, string? str, Encoding? encoding = null)
    {
        if (str is null)
        {
            writer.WriteNull();
            return;
        }

        encoding ??= Encoding.Unicode;
        
        writer.Write(encoding.CodePage);
        writer.WriteBytes(encoding.GetBytes(str));
    }

    private static void WriteNull(this BinaryWriter writer) => writer.Write((byte)0);

    private static void WriteBytes(this BinaryWriter writer, byte[] array)
    {
        writer.Write(array.Length);
        writer.Write(array);
    }

    private static void WriteNullable(this BinaryWriter writer, string? str)
    {
        if (str is null)
        {
            writer.WriteNull();
            return;
        }
        
        writer.Write(str);
    }
}