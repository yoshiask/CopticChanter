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
    public static void WriteDefinition(Stream stream, IDefinition obj)
    {
        BinaryWriter writer = new(stream);
        WriteObject(writer, obj);
    }
    
    public static void WriteObject(this BinaryWriter writer, object? obj)
    {
        var defCode = obj switch
        {
            null                        => HyperspeedDefinitionCode.Null,
            Doc _                       => HyperspeedDefinitionCode.Doc,
            Stanza _                    => HyperspeedDefinitionCode.Stanza,
            Section _                   => HyperspeedDefinitionCode.Section,
            Run _                       => HyperspeedDefinitionCode.Run,
            Comment _                   => HyperspeedDefinitionCode.Comment,
            SimpleContent _             => HyperspeedDefinitionCode.SimpleContent,
            IScript<object> _           => HyperspeedDefinitionCode.Script,
            Variable _                  => HyperspeedDefinitionCode.Variable,
            TranslationCollection _     => HyperspeedDefinitionCode.Translations,
            TranslationRunCollection _  => HyperspeedDefinitionCode.TranslationRuns,
            
            _ => throw new ArgumentOutOfRangeException(nameof(obj), obj.GetType().Name, "")
        };
        
        writer.Write((ushort)defCode);
        if (obj is null)
            return;

        if (obj is IDefinition def)
        {
            writer.WriteNullable(def.Key);
        }
        if (obj is Doc doc)
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
                writer.WriteObject(translation);
            
            // TODO: Optimize direct definitions, only keep scripts and variables
            writer.Write(doc.DirectDefinitions.Count);
            foreach (var directDefinitions in doc.DirectDefinitions)
                writer.WriteObject(directDefinitions);
        }

        if (obj is IScript<object> script)
        {
            writer.Write(script.TypeId);
            writer.WriteEncodedString(script.ScriptBody);
        }
        if (obj is IMultilingual multilingual)
        {
            var elemLanguage = multilingual.Language;
            var elemFont = multilingual.Font;

            var multiDef = (IDefinition)obj;
            if (multiDef.Parent is not null)
            {
                var parentLanguage = multiDef.Parent.GetLanguage();
                if (!LanguageInfo.IsNullOrDefault(parentLanguage) && parentLanguage == elemLanguage)
                    elemLanguage = null;
                    
                var parentFont = multiDef.Parent.GetFont();
                if (parentFont is not null && parentFont == elemFont)
                    elemFont = null;
            }
            
            multilingual.HandleFont();
            
            writer.WriteNullable(elemLanguage?.ToString());
            writer.WriteNullable(elemFont);
        }
        if (obj is IContent content)
        {
            content.HandleCommands();
            
            // TODO: Optimize inlines
            writer.WriteEncodedString(content.GetText());
        }
        if (obj is IContentCollectionContainer contentCollection)
        {
            writer.WriteObject(contentCollection.Source);
            
            writer.Write(contentCollection.Children.Count);
            foreach (var child in contentCollection.Children)
                writer.WriteObject(child);
        }

        // Serialize class-specific properties
        switch (obj)
        {
            case Section section:
                writer.WriteObject(section.Title);
                writer.WriteEncodedString(section.Title?.GetText());
                break;
            
            case Run run:
                writer.WriteEncodedString(run.Text);
                break;
            
            case TranslationRunCollection runCollection:
                writer.Write(runCollection.Count);
                foreach (var translationRun in runCollection)
                    writer.WriteObject(translationRun);
                break;

            case Variable variable:
                writer.Write(variable.Label);
                writer.WriteObject(variable.DefaultValue);
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