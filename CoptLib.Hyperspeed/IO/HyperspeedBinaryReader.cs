using System;
using System.IO;
using System.Text;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Scripting;
using CoptLib.Writing;

namespace CoptLib.Hyperspeed.IO;

public class HyperspeedBinaryReader : BinaryReader
{
    public ILoadContext? Context { get; }
    
    public HyperspeedBinaryReader(Stream input, ILoadContext? context = null) : base(input)
    {
        Context = context;
    }

    public Doc ReadDoc()
    {
        return ReadObject<Doc>() ?? throw new InvalidDataException("Hyperspeed file was not a document.");
    }
    
    public IDefinition? ReadDefinition(IDefinition? parent = null) => ReadObject<IDefinition>(parent);

    public T? ReadObject<T>(IDefinition? parent = null)
    {
        var obj = ReadObject(parent);
        return obj is null ? default : (T)obj;
    }
    
    public object? ReadObject(IDefinition? parent = null)
    {
        var objCode = (HyperspeedObjectCode)ReadUInt16();
        if (objCode is HyperspeedObjectCode.Null)
            return null;
        
        var key = ReadNullableString();
        
        object obj = objCode switch
        {
            HyperspeedObjectCode.Doc => new Doc(Context),
            HyperspeedObjectCode.Stanza => new Stanza(parent),
            HyperspeedObjectCode.Section => new Section(parent),
            HyperspeedObjectCode.Run => new Run(parent),
            HyperspeedObjectCode.Comment => new Comment(parent),
            HyperspeedObjectCode.SimpleContent => new SimpleContent(null, parent),
            HyperspeedObjectCode.Script => ReadScript(),
            HyperspeedObjectCode.Variable => new Variable(),
            HyperspeedObjectCode.Translations => new TranslationCollection(),
            HyperspeedObjectCode.TranslationRuns => new TranslationRunCollection(),
            
            _ => throw new ArgumentOutOfRangeException(nameof(objCode), objCode, "Invalid object code")
        };

        var def = obj as IDefinition;
        if (def is not null)
        {
            def.Key = key;
            def.Parent = parent;
        }
        
        if (obj is Doc doc)
        {
            doc.Name = ReadEncodedString()!;
            doc.Author = ReadObject<Author>();
            
            var translationCount = ReadInt32();
            for (int t = 0; t < translationCount; t++)
            {
                var translation = ReadObject<ContentPart>(def)!;
                doc.Translations.Children.Add(translation);
            }
            
            var definitionCount = ReadInt32();
            for (int d = 0; d < definitionCount; d++)
            {
                doc.AddDefinition(ReadObject<IDefinition>(def)!);
            }
        }
        if (obj is IMultilingual multilingual)
        {
            var language = ReadNullableString();
            if (language is not null)
                multilingual.Language = LanguageInfo.Parse(language);

            multilingual.Font = ReadNullableString();
        }
        if (obj is IContent content)
        {
            // TODO: Optimize inlines
            content.SourceText = ReadEncodedString()!;
        }
        if (obj is IContentCollectionContainer contentCollection)
        {
            contentCollection.Source = ReadObject<SimpleContent>(contentCollection);
            
            var contentCollectionCount = ReadInt32();
            for (int p = 0; p < contentCollectionCount; p++)
            {
                var part = ReadObject<ContentPart>(contentCollection)!;
                contentCollection.Children.Add(part);
            }
        }

        // Serialize class-specific properties
        switch (obj)
        {
            case Section section:
                var sectionTitle = ReadObject<IContent>(section);
                section.SetTitle(sectionTitle);
                break;

            case Run run:
                run.Text = ReadEncodedString()!;
                break;
            
            case TranslationRunCollection runCollection:
                var runCollectionCount = ReadInt32();
                for (int p = 0; p < runCollectionCount; p++)
                {
                    var translationRun = ReadObject<Run>(runCollection)!;
                    runCollection.AddRun(translationRun);
                }
                break;
            
            case Variable variable:
                variable.Label = ReadString();
                variable.DefaultValue = ReadObject(variable);
                variable.Configurable = ReadBoolean();
                break;
            
            case Author author:
                author.FullName = ReadEncodedString();
                author.PhoneNumber = ReadNullableString();
                author.Email = ReadNullableString();
                author.Website = ReadNullableString();
                break;
        }

        return obj;
    }

    public IDefinition ReadScript()
    {
        var typeId = ReadString()!;
        var scriptBody = ReadEncodedString()!;
        return (IDefinition)ScriptingEngine.CreateScript(typeId, scriptBody);
    }
    
    public string? ReadEncodedString()
    {
        if (PeekChar() == 0)
        {
            ++BaseStream.Position;
            return null;
        }

        var encoding = Encoding.GetEncoding(ReadInt32());
        var bytes = ReadBytes();
        return encoding.GetString(bytes);
    }

    public byte[] ReadBytes()
    {
        var length = ReadInt32();
        return ReadBytes(length);
    }

    public string? ReadNullableString()
    {
        if (PeekChar() == 0)
        {
            ++BaseStream.Position;
            return null;
        }

        return ReadString();
    }
}