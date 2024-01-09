using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Sequences;
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

    public void WriteEx(object? obj)
    {
        switch (obj)
        {
            case DocSet set:
                Write(set);
                break;
            
            case IDefinition def:
                Write(def);
                break;
            
            case HyperspeedObjectCode code:
                Write(code);
                break;
            
            case Author author:
                Write(author);
                break;
            
            case byte[] bytes:
                WriteBytes(bytes);
                break;
            
            default:
                throw new ArgumentException($"'{obj?.GetType()}' is not a Hyperseed object.");
        }
    }
    
    public void Write(IDefinition? def)
    {
        if (WriteNullIfNull(def))
            return;
        
        var defCode = def switch
        {
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
            
            _ => throw new ArgumentOutOfRangeException(nameof(def), def.GetType().Name, "")
        };
        
        Write(defCode);

        if (def is IScript<object?> script)
        {
            // Make sure we write the type ID first, it's effectively an extension
            // of the Hyperspeed object code and is required to know which script
            // type to instantiate when reading.
            Write(script.TypeId);

            // NOTE: Support compiling script in the future
            if (false)
            {
                Write(true);
            }
            else
            {
                Write(false);
                WriteEncodedString(script.ScriptBody);
            }
        }
        
        WriteNullable(def.Key);
        
        if (def is Doc doc)
        {
            WriteEncodedString(doc.Name);
            Write(doc.Author);

            doc.ApplyTransforms();
            
            Write(doc.Translations.Children.Count);
            foreach (var translation in doc.Translations.Children)
                Write(translation);
            
            // TODO: Optimize direct definitions, only keep scripts and variables
            Write(doc.DirectDefinitions.Count);
            foreach (var directDefinitions in doc.DirectDefinitions)
                Write(directDefinitions);
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
            
            WriteNullable(elemLanguage?.ToString());
            WriteNullable(elemFont);
        }
        if (def is IContent content)
        {
            content.HandleCommands();
            
            // TODO: Optimize inlines
            WriteEncodedString(content.GetText());
        }
        if (def is IContentCollectionContainer contentCollection)
        {
            Write(contentCollection.Source);
            
            Write(contentCollection.Children.Count);
            foreach (var child in contentCollection.Children)
                Write(child);
        }

        // Serialize class-specific properties
        switch (def)
        {
            case Section section:
                Write(section.Title);
                break;
            
            case Run run:
                WriteEncodedString(run.Text);
                break;
            
            case TranslationRunCollection runCollection:
                Write(runCollection.Count);
                foreach (var translationRun in runCollection)
                    Write(translationRun);
                break;

            case Variable variable:
                Write(variable.Label);
                Write((IDefinition?) variable.DefaultValue);
                Write(variable.Configurable);
                break;
        }
        
        Flush();
    }

    public void Write(Author? author)
    {
        if (WriteNullIfNull(author))
            return;
        
        WriteEncodedString(author.FullName);
        WriteNullable(author.PhoneNumber);
        WriteNullable(author.Email);
        WriteNullable(author.Website);
    }
    
    public void WriteEncodedString(string? str, Encoding? encoding = null)
    {
        if (WriteNullIfNull(str))
            return;

        encoding ??= Encoding.Unicode;
        
        Write(encoding.CodePage);
        WriteBytes(encoding.GetBytes(str));
    }

    public void WriteNull() => Write(HyperspeedObjectCode.Null);

    public bool WriteNullIfNull([NotNullWhen(false)] object? obj)
    {
        if (obj is null)
        {
            WriteNull();
            return true;
        }

        return false;
    }

    public void WriteBytes(byte[] array)
    {
        Write(array.Length);
        Write(array);
    }

    public void WriteNullable(string? str)
    {
        if (WriteNullIfNull(str))
            return;
        
        Write(str);
    }

    public void WriteNullable(int? num)
    {
        if (WriteNullIfNull(num))
            return;
        
        Write(num.Value);
    }

    public void Write(HyperspeedObjectCode code) => Write((ushort)code);
    public void Write(HyperspeedSequenceNodeCode code) => Write((byte)code);

    public void Write(DocSet set)
    {
        Write(HyperspeedObjectCode.Set);
        Write(set.Key!);
        Write(set.Name);
        Write(set.Author);
        
        // Reserve space for the jump table.
        // This section is an array of longs, where each
        // item is the position of an included doc.
        var jumpTableLength = set.IncludedDocs.Count * sizeof(long);
        Write(jumpTableLength);
        var jumpTableStart = BaseStream.Position;
        BaseStream.Position += jumpTableLength;
        
        // Start writing included docs, keeping track of the starting
        // offset of each doc so we can later write this to the jump table.
        var jumpTableEntries = new long[set.IncludedDocs.Count];
        for (int d = 0; d < set.IncludedDocs.Count; d++)
        {
            jumpTableEntries[d] = BaseStream.Position;
            Write(set.IncludedDocs[d]);
        }
        
        // Move back to the jump table and fill in the positions
        var jumpTableBytes = new byte[jumpTableLength];
        Buffer.BlockCopy(jumpTableEntries, 0, jumpTableBytes, 0, jumpTableLength);
        BaseStream.Position = jumpTableStart;
        Write(jumpTableBytes);
    }

    public void Write(ISequence sequence)
    {
        Write(HyperspeedObjectCode.Sequence);
        WriteNullable(sequence.Key);
        WriteNullable(sequence.Name);
        Write(sequence.RootNodeId);

        var nodeCount = sequence.Nodes.Count;
        Write(nodeCount);
        
        // Reserve space for ID -> offset map
        var offsetMapLength = nodeCount * (sizeof(int) + sizeof(long));
        var offsetMapStart = BaseStream.Position;
        BaseStream.Position += offsetMapLength;
        var offsetMapEnd = BaseStream.Position;
        
        Dictionary<int, long> offsetMap = new(nodeCount);
        foreach (var node in sequence.Nodes.Values)
        {
            // Note where this node begins
            var nodeOffset = BaseStream.Position - offsetMapEnd;
            offsetMap.Add(node.Id, nodeOffset);
            
            // Write the sequence node
            Write(node);
        }
        
        // Move back to the offset map and write the entries
        BaseStream.Position = offsetMapStart;
        foreach (var kvp in offsetMap)
        {
            Write(kvp.Key);
            Write(kvp.Value);
        }
    }

    public void Write(SequenceNode node)
    {
        Write(node.Id);
        WriteNullable(node.DocumentKey);

        if (node is ConstantSequenceNode constNode)
        {
            Write(node is EndSequenceNode
                ? HyperspeedSequenceNodeCode.End
                : HyperspeedSequenceNodeCode.Constant);
            WriteNullable(constNode.NextNodeId);
        }
        else if (node is NullSequenceNode)
        {
            Write(HyperspeedSequenceNodeCode.Null);
        }
        else if (node is ScriptedSequenceNode scriptedNode)
        {
            Write(HyperspeedSequenceNodeCode.Scripted);
            Write(scriptedNode.NextDocCommand as IDefinition);
        }
        else
        {
            throw new NotSupportedException($"Hyperspeed does not support '{node.GetType()}' sequence nodes.");
        }
    }
}