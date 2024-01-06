using System;
using System.IO;
using System.Text;
using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Scripting;
using CoptLib.Writing;

namespace CoptLib.Hyperspeed.IO;

public class HyperspeedBinaryWriter : BinaryWriter
{
    public HyperspeedBinaryWriter(Stream output) : base(output)
    {
    }

    protected HyperspeedBinaryWriter() : base()
    {
    }
    
    public void WriteObject(object? obj)
    {
        var defCode = obj switch
        {
            null                        => HyperspeedObjectCode.Null,
            Doc _                       => HyperspeedObjectCode.Doc,
            Stanza _                    => HyperspeedObjectCode.Stanza,
            Section _                   => HyperspeedObjectCode.Section,
            Run _                       => HyperspeedObjectCode.Run,
            Comment _                   => HyperspeedObjectCode.Comment,
            SimpleContent _             => HyperspeedObjectCode.SimpleContent,
            IScript<object> _           => HyperspeedObjectCode.Script,
            Variable _                  => HyperspeedObjectCode.Variable,
            TranslationCollection _     => HyperspeedObjectCode.Translations,
            TranslationRunCollection _  => HyperspeedObjectCode.TranslationRuns,
            
            _ => throw new ArgumentOutOfRangeException(nameof(obj), obj.GetType().Name, "")
        };
        
        Write((ushort)defCode);
        if (obj is null)
            return;

        if (obj is IDefinition def)
        {
            WriteNullable(def.Key);
        }
        if (obj is Doc doc)
        {
            WriteEncodedString(doc.Name);

            if (doc.Author is null)
            {
                Write(false);
            }
            else
            {
                Write(true);
                WriteEncodedString(doc.Author.FullName);
                WriteNullable(doc.Author.PhoneNumber);
                WriteNullable(doc.Author.Email);
                WriteNullable(doc.Author.Website);
            }

            doc.ApplyTransforms();
            
            Write(doc.Translations.Children.Count);
            foreach (var translation in doc.Translations.Children)
                WriteObject(translation);
            
            // TODO: Optimize direct definitions, only keep scripts and variables
            Write(doc.DirectDefinitions.Count);
            foreach (var directDefinitions in doc.DirectDefinitions)
                WriteObject(directDefinitions);
        }

        if (obj is IScript<object> script)
        {
            Write(script.TypeId);
            WriteEncodedString(script.ScriptBody);
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
            
            WriteNullable(elemLanguage?.ToString());
            WriteNullable(elemFont);
        }
        if (obj is IContent content)
        {
            content.HandleCommands();
            
            // TODO: Optimize inlines
            WriteEncodedString(content.GetText());
        }
        if (obj is IContentCollectionContainer contentCollection)
        {
            WriteObject(contentCollection.Source);
            
            Write(contentCollection.Children.Count);
            foreach (var child in contentCollection.Children)
                WriteObject(child);
        }

        // Serialize class-specific properties
        switch (obj)
        {
            case Section section:
                WriteObject(section.Title);
                break;
            
            case Run run:
                WriteEncodedString(run.Text);
                break;
            
            case TranslationRunCollection runCollection:
                Write(runCollection.Count);
                foreach (var translationRun in runCollection)
                    WriteObject(translationRun);
                break;

            case Variable variable:
                Write(variable.Label);
                WriteObject(variable.DefaultValue);
                Write(variable.Configurable);
                break;
        }
        
        Flush();
    }

    public void WriteEncodedString(string? str, Encoding? encoding = null)
    {
        if (str is null)
        {
            WriteNull();
            return;
        }

        encoding ??= Encoding.Unicode;
        
        Write(encoding.CodePage);
        WriteBytes(encoding.GetBytes(str));
    }

    public void WriteNull() => Write((byte)0);

    public void WriteBytes(byte[] array)
    {
        Write(array.Length);
        Write(array);
    }

    public void WriteNullable(string? str)
    {
        if (str is null)
        {
            WriteNull();
            return;
        }
        
        Write(str);
    }
}