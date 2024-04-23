using System;
using CoptLib.Models;
using CoptLib.Writing;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CommunityToolkit.Diagnostics;
using CoptLib.Models.Text;
using CoptLib.Scripting.Typed;
using CoptLib.Extensions;

namespace CoptLib.IO;

public static class DocReader
{
    /// <summary>
    /// Deserializes and returns a DocXML file.
    /// </summary>
    /// <param name="path">The path to the XML file</param>
    /// <param name="context">The context to load the document into.</param>
    /// <returns></returns>
    /// <remarks>
    /// Do not use when broad filesystem access is not available.
    /// Instead, load the file manually and use <see cref="ReadDocXml(string, ILoadContext)"/>. 
    /// </remarks>
    public static Doc ReadDocXml(string path, ILoadContext? context = null)
        => ParseDocXml(XDocument.Load(path), context);

    /// <summary>
    /// Deserializes and returns a DocXML file.
    /// </summary>
    /// <param name="file">A Stream of the XML file</param>
    /// <param name="context">The context to load the document into.</param>
    /// <returns></returns>
    public static Doc ReadDocXml(Stream file, ILoadContext? context = null)
        => ParseDocXml(XDocument.Load(file, LoadOptions.None), context);

    /// <summary>
    /// Parses the XML string into a <see cref="Doc"/>.
    /// </summary>
    public static Doc ParseDocXml(string xml, ILoadContext? context = null)
        => ParseDocXml(XDocument.Parse(xml), context);

    /// <summary>
    /// Parses the XML document tree into a <see cref="Doc"/>.
    /// </summary>
    public static Doc ParseDocXml(XDocument xml, ILoadContext? context = null)
    {
        Guard.IsNotNull(xml.Root);

        context ??= new LoadContext();
            
        // The actual content can't be directly deserialized, so it needs to be manually parsed
        Doc doc = new(context)
        {
            Name = xml.Root.Element(nameof(doc.Name))!.Value,
            Key = xml.Root.Element(nameof(doc.Key))?.Value,
        };

        // BACKCOMPAT: Support documents that use the Uuid element name
        doc.Key ??= xml.Root.Element("Uuid")?.Value;

        var defsElem = xml.Root.Element("Definitions");
        if (defsElem != null)
        {
            var defs = ParseDefinitionCollection(defsElem.Elements(), doc);
            doc.DirectDefinitions = defs;
        }

        var transsElem = xml.Root.Element(nameof(doc.Translations));
        if (transsElem != null)
        {
            foreach (var def in ParseDefinitionCollection(transsElem.Elements(), doc))
                if (def is ContentPart translation)
                    doc.Translations.Children.Add(translation);
        }

        var authorElem = xml.Root.Element("Author");
        if (authorElem != null)
            doc.Author = Author.DeserializeElement(authorElem);
        
        context.AddDoc(doc);

        return doc;
    }

    public static List<IDefinition> ParseDefinitionCollection(IEnumerable<XElement> elements, IDefinition? parent)
    {
        var doc = parent as Doc ?? parent?.DocContext;
        
        List<IDefinition> defs = new();

        foreach (var defElem in elements)
        {
            var def = ParseDefinitionXml(defElem, parent);
            if (def == null)
                continue;

            if (def.Key != null)
                doc?.AddDefinition(def);
            defs.Add(def);
        }

        return defs;
    }

    public static IDefinition? ParseDefinitionXml(XElement elem, IDefinition? parent)
    {
        IDefinition? def = null;
        string defElemName = elem.Name.LocalName;
        var doc = parent as Doc ?? parent?.DocContext;

        if (defElemName == nameof(Stanza))
        {
            def = new Stanza(parent);
        }
        else if (defElemName is nameof(Section) or "Translation")
        {
            Section section = new(parent);

            string? title = elem.Attribute(nameof(section.Title))?.Value;
            if (title != null)
                section.SetTitle(title);

            def = section;
        }
        else if (defElemName is nameof(PartReference) or "Reference")
        {
            PartReference reference = new(parent);

            var referenceText = elem.Value;
            if (referenceText != null)
                reference.Source = new(referenceText, reference);

            def = reference;
        }
        else if (defElemName == nameof(Run))
        {
            def = new Run(elem.Value, parent);
        }
        else if (defElemName == nameof(Comment))
        {
            Comment comment = new(parent);

            var commentTypeStr = elem.Attribute("Type")?.Value;
            if (commentTypeStr is not null && Enum.TryParse(commentTypeStr, out CommentType commentType))
                comment.Type = commentType;

            if (comment.Type == CommentType.Role)
            {
                var roleId = comment.SourceText = elem.Value;
                if ((comment.DocContext?.Context.TryLookupDefinition(roleId, out var roleDef) ?? false)
                    && roleDef is RoleInfo role)
                {
                    comment.Inlines.Add(role.GetByLanguage(comment.GetLanguage()));
                    role.References.Add(comment);
                }
            }

            def = comment;
        }
        else if (defElemName == "String")
        {
            def = new SimpleContent(null, parent);
        }
        else if (defElemName == "Script")
        {
            // Default to csharp-def (DotNetDefinitionScript) for backward-compatibility.
            // Since it's the default, make sure it's always registered.
            DotNetDefinitionScript.Register();
            
            var scriptTypeId = elem.Attribute("Type")?.Value ?? "csharp-def";

            var script = Scripting.ScriptingEngine.CreateScript(
                scriptTypeId, elem.Value);

            if (script is not IDefinition defScript)
                throw new InvalidDataException($"Script of type '{scriptTypeId}' must implement " +
                                               $"{nameof(IDefinition)} to be defined directly in a document.");
                
            def = defScript;
        }
        else if (defElemName == nameof(Variable))
        {
            var configurableStr = elem.Attribute("Configurable")?.Value;
            Variable variable = new()
            {
                Label = elem.Attribute(nameof(variable.Label))?.Value,
                DefaultValue = elem.Attribute(nameof(variable.DefaultValue))?.Value,
                Configurable = configurableStr is not null && bool.Parse(configurableStr),
            };
            def = variable;
        }
        else if (defElemName == "Translations")
        {
            def = new TranslationCollection(parent: parent);
        }
        else if (defElemName == "Doc")
        {
            def = new Doc
            {
                Key = elem.Attribute(nameof(Doc.Key))?.Value,
                Name = elem.Attribute(nameof(Doc.Name))!.Value,
            };
        }

        if (def is null)
            return null;
        
        def.DocContext = doc;
        def.Parent = parent;
        def.Key = elem.Attribute(nameof(def.Key))?.Value;

        // Not every IDefinition is explicitly defined,
        // but since this branch only runs when we already
        // have an XML element, this is a safe assumption.
        def.IsExplicitlyDefined = true;
                
        if (def is IContent content)
        {
            content.SourceText = elem.Value;
        }
        
        if (def is IMultilingual multilingual)
        {
            multilingual.Font = elem.Attribute(nameof(multilingual.Font))?.Value;

            var langVal = elem.Attribute(nameof(multilingual.Language))?.Value;
            if (!string.IsNullOrEmpty(langVal))
                multilingual.Language = LanguageInfo.Parse(langVal!);

            if (parent is IMultilingual parentMultilingual)
            {
                multilingual.Font ??= parentMultilingual.Font;
                if (multilingual.Language.IsDefault())
                    multilingual.Language = parentMultilingual.Language;
            }
        }

        if (def is ContentPart && elem.Attribute("Role") != null)
            throw new Exception("Roles are now their own comment type, rather than a property on ContentPart.");
            
        if (def is IContentCollectionContainer contentCollection and IDefinition defCC)
        {
            // Parse elements, remove anything not a ContentPart
            var defColl = ParseDefinitionCollection(elem.Elements(), defCC)
                .OfType<ContentPart>();
            contentCollection.Children.AddRange(defColl);
        }

        if (def is TranslationCollection translationCollection and IDefinition defTC)
        {
            var translations = ParseDefinitionCollection(elem.Elements(), defTC)
                .OfType<IMultilingual>()
                .ToImmutableArray();

            if (translations.All(t => t is Run))
            {
                TranslationRunCollection runs = new(def.Key, def.Parent);
                runs.AddRuns(translations.OfType<Run>());
                def = runs;
            }
            else
            {
                translationCollection.AddRange(translations);
            }
        }

        return def;
    }
}