using System;
using System.IO;
using System.Text;
using CoptLib.Extensions;
using CoptLib.Hyperspeed.Memory;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Scripting;
using CoptLib.Writing;
using MessagePack;

namespace CoptLib.Hyperspeed.IO;

public static class HyperspeedDocWriter
{
    public static void SerializeDefinition(Stream stream, IDefinition def)
    {
        StreamBufferWriter buffer = new(stream);

        MessagePackWriter writer = new(buffer);
        SerializeDefinition(writer, def);
    }
    
    public static void SerializeDefinition(MessagePackWriter msg, IDefinition def)
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
        
        msg.Write((uint)defCode);
        msg.Write(def.Key);

        if (def is Doc doc)
        {
            msg.WriteEncodedString(doc.Name);

            if (doc.Author is null)
            {
                msg.Write(false);
            }
            else
            {
                msg.Write(true);
                msg.WriteEncodedString(doc.Author.FullName);
                msg.Write(doc.Author.PhoneNumber);
                msg.Write(doc.Author.Email);
                msg.Write(doc.Author.Website);
            }

            doc.ApplyTransforms();
            
            msg.WriteArrayHeader(doc.Translations.Children.Count);
            foreach (var translation in doc.Translations.Children)
                SerializeDefinition(msg, translation);
            
            // TODO: Optimize direct definitions, only keep scripts and variables
            msg.WriteArrayHeader(doc.DirectDefinitions.Count);
            foreach (var directDefinitions in doc.DirectDefinitions)
                SerializeDefinition(msg, directDefinitions);
        }

        if (def is IScript<object> script)
        {
            msg.Write(script.TypeId);
            msg.WriteEncodedString(script.ScriptBody);
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
            
            msg.Write(elemLanguage?.ToString());
            msg.Write(elemFont);
        }
        if (def is IContent content)
        {
            content.HandleCommands();
            
            // TODO: Optimize inlines
            msg.WriteEncodedString(content.GetText());
        }
        if (def is IContentCollectionContainer contentCollection)
        {
            msg.WriteEncodedString(contentCollection.Source?.ToString());
            
            msg.WriteArrayHeader(contentCollection.Children.Count);
            foreach (var child in contentCollection.Children)
                SerializeDefinition(msg, child);
        }

        // Serialize class-specific properties
        switch (def)
        {
            case Section section:
                msg.WriteEncodedString(section.Title?.ToString());
                break;

            case Variable variable:
                msg.Write(variable.Label);
                msg.WriteEncodedString(variable.DefaultValue?.ToString());
                msg.Write(variable.Configurable);
                break;
        }
        
        msg.Flush();
    }

    private static void WriteEncodedString(this MessagePackWriter msg, string? str, Encoding? encoding = null)
    {
        if (str is null)
        {
            msg.WriteNil();
            return;
        }

        encoding ??= Encoding.Unicode;
        
        msg.Write(encoding.CodePage);
        msg.Write(encoding.GetBytes(str));
        msg.Flush();
    }
}