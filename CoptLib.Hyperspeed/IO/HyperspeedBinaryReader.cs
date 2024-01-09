using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Sequences;
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
        if (TryPeekHyperspeedObjectCode(out var code) && code != HyperspeedObjectCode.Doc)
            throw new InvalidDataException($"Hyperspeed object at 0x{BaseStream.Position:X} was not a document.");
        
        return ReadDefinition<Doc>()!;
    }

    public DocSet ReadSet()
    {
        if (ReadHyperspeedObjectCode() != HyperspeedObjectCode.Set)
            throw new InvalidDataException($"Hyperspeed object at 0x{BaseStream.Position:X} was not a set.");

        var key = ReadString();
        var name = ReadString();
        var author = ReadAuthor();

        // Read jump table
        var jumpTableLength = ReadInt32();
        var jumpTableBytes = ReadBytes(jumpTableLength);
        var docCount = jumpTableLength / sizeof(long);
        var jumpTableEntries = new long[docCount];
        Buffer.BlockCopy(jumpTableBytes, 0, jumpTableEntries, 0, jumpTableLength);
        
        // Read docs
        List<Doc> docs = new(docCount);
        for (int d = 0; d < docCount; d++)
        {
            var doc = ReadDoc();
            docs.Add(doc);
        }
        
        DocSet set = new(key, name, docs)
        {
            Author = author,
        };
        return set;
    }

    public IReadOnlySequence ReadSequence()
    {
        if (ReadHyperspeedObjectCode() != HyperspeedObjectCode.Sequence)
            throw new InvalidDataException($"Hyperspeed object at 0x{BaseStream.Position:X} was not a sequence.");
        
        var key = ReadNullableString();
        var name = ReadNullableString();
        var rootNodeId = ReadInt32();
        
        // Read offset mapping
        var nodeCount = ReadInt32();
        Dictionary<int, long> offsetMap = new(nodeCount);
        for (int n = 0; n < nodeCount; n++)
        {
            var id = ReadInt32();
            var offset = ReadInt64();
            offsetMap.Add(id, offset);
        }

        MemoryStream nodesStream = new();
        BaseStream.CopyTo(nodesStream);
        nodesStream.Position = 0;
        
        HyperspeedSequence sequence = new(key, name, Context!, rootNodeId, offsetMap, nodesStream);
        return sequence;
    }

    public T? ReadDefinition<T>(IDefinition? parent = null) where T : IDefinition
    {
        var obj = ReadDefinition(parent);
        return obj is null ? default : (T)obj;
    }
    
    public IDefinition? ReadDefinition(IDefinition? parent = null)
    {
        var objCode = (HyperspeedObjectCode)ReadUInt16();
        if (objCode is HyperspeedObjectCode.Null)
            return null;
        
        var key = ReadNullableString();
        
        IDefinition def = objCode switch
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

        def.Key = key;
        def.Parent = parent;
        
        if (def is Doc doc)
        {
            doc.Name = ReadEncodedString()!;
            doc.Author = ReadAuthor();
            
            var translationCount = ReadInt32();
            for (int t = 0; t < translationCount; t++)
            {
                var translation = ReadDefinition<ContentPart>(def)!;
                doc.Translations.Children.Add(translation);
            }
            
            var definitionCount = ReadInt32();
            for (int d = 0; d < definitionCount; d++)
            {
                doc.AddDefinition(ReadDefinition<IDefinition>(def)!);
            }
        }
        if (def is IMultilingual multilingual)
        {
            var language = ReadNullableString();
            if (language is not null)
                multilingual.Language = LanguageInfo.Parse(language);

            multilingual.Font = ReadNullableString();
        }
        if (def is IContent content)
        {
            // TODO: Optimize inlines
            content.SourceText = ReadEncodedString()!;
        }
        if (def is IContentCollectionContainer contentCollection)
        {
            contentCollection.Source = ReadDefinition<SimpleContent>(contentCollection);
            
            var contentCollectionCount = ReadInt32();
            for (int p = 0; p < contentCollectionCount; p++)
            {
                var part = ReadDefinition<ContentPart>(contentCollection)!;
                contentCollection.Children.Add(part);
            }
        }

        // Serialize class-specific properties
        switch (def)
        {
            case Section section:
                var sectionTitle = ReadDefinition<IContent>(section);
                section.SetTitle(sectionTitle);
                break;

            case Run run:
                run.Text = ReadEncodedString()!;
                break;
            
            case TranslationRunCollection runCollection:
                var runCollectionCount = ReadInt32();
                for (int p = 0; p < runCollectionCount; p++)
                {
                    var translationRun = ReadDefinition<Run>(runCollection)!;
                    runCollection.AddRun(translationRun);
                }
                break;
            
            case Variable variable:
                variable.Label = ReadString();
                variable.DefaultValue = ReadDefinition(variable);
                variable.Configurable = ReadBoolean();
                break;
        }

        return def;
    }

    public IDefinition ReadScript()
    {
        var typeId = ReadString()!;

        var isCompiled = ReadBoolean();
        if (isCompiled)
        {
            throw new NotSupportedException($"'{typeId}' does not support compiled scripts.");
        }
        else
        {
            var scriptBody = ReadEncodedString()!;
            return (IDefinition)ScriptingEngine.CreateScript(typeId, scriptBody);
        }
    }

    public SequenceNode ReadSequenceNode(IDefinition? parent = null)
    {
        var id = ReadInt32();
        var documentKey = ReadNullableString();
        var code = ReadHyperspeedSequenceNodeCode();
        SequenceNode node;

        if (code == HyperspeedSequenceNodeCode.Null)
        {
            node = new NullSequenceNode(id);
        }
        else if (code == HyperspeedSequenceNodeCode.Constant)
        {
            var nextNodeId = ReadNullableInt32();
            node = new ConstantSequenceNode(id, documentKey, nextNodeId);
        }
        else if (code == HyperspeedSequenceNodeCode.End)
        {
            _ = ReadNullableInt32();
            node = new EndSequenceNode(id, documentKey);
        }
        else if (code == HyperspeedSequenceNodeCode.Scripted)
        {
            var script = ReadDefinition(parent) as ICommandOutput<object>;
            node = new ScriptedSequenceNode(id, documentKey, script!);
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(code), code, "Invalid node code");
        }

        return node;
    }

    public Author? ReadAuthor()
    {
        var code  = ReadHyperspeedObjectCode();
        if (code == HyperspeedObjectCode.Null)
            return null;
        
        if (code != HyperspeedObjectCode.Author)
            throw new InvalidDataException($"Hyperspeed object at 0x{BaseStream.Position:X} was not author metadata.");
        
        return new()
        {
            FullName = ReadEncodedString(),
            PhoneNumber = ReadNullableString(),
            Email = ReadNullableString(),
            Website = ReadNullableString()
        };
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

    public int? ReadNullableInt32()
    {
        if (PeekChar() == 0)
        {
            ++BaseStream.Position;
            return null;
        }

        return ReadInt32();
    }

    public HyperspeedObjectCode ReadHyperspeedObjectCode() => (HyperspeedObjectCode)ReadUInt16();
    public HyperspeedSequenceNodeCode ReadHyperspeedSequenceNodeCode() => (HyperspeedSequenceNodeCode)ReadByte();

    public bool TryPeekHyperspeedObjectCode(out HyperspeedObjectCode code)
    {
        if (!BaseStream.CanSeek)
        {
            code = HyperspeedObjectCode.Null;
            return false;
        }
        
        var currentPosition = BaseStream.Position;
        code = ReadHyperspeedObjectCode();
        
        BaseStream.Position = currentPosition;
        return true;
    }
}