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
    /// Instead, load the file manually and use <see cref="ReadDocXml(string,CoptLib.IO.LoadContextBase)"/>. 
    /// </remarks>
    public static Doc ReadDocXml(string path, LoadContextBase? context = null)
        => ParseDocXml(XDocument.Load(path), context);

    /// <summary>
    /// Deserializes and returns a DocXML file.
    /// </summary>
    /// <param name="file">A Stream of the XML file</param>
    /// <param name="context">The context to load the document into.</param>
    /// <returns></returns>
    public static Doc ReadDocXml(Stream file, LoadContextBase? context = null)
        => ParseDocXml(XDocument.Load(file, LoadOptions.None), context);

    /// <summary>
    /// Parses the XML string into a <see cref="Doc"/>.
    /// </summary>
    public static Doc ParseDocXml(string xml, LoadContextBase? context = null)
        => ParseDocXml(XDocument.Parse(xml), context);

    /// <summary>
    /// Parses the XML document tree into a <see cref="Doc"/>.
    /// </summary>
    public static Doc ParseDocXml(XDocument xml, LoadContextBase? context = null)
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
            var defs = ParseDefinitionCollection(defsElem.Elements(), doc, null);
            doc.DirectDefinitions = defs;
        }

        var transsElem = xml.Root.Element(nameof(doc.Translations));
        if (transsElem != null)
        {
            foreach (var def in ParseDefinitionCollection(transsElem.Elements(), doc, null))
                if (def is ContentPart translation)
                    doc.Translations.Children.Add(translation);
        }

        var authorElem = xml.Root.Element("Author");
        if (authorElem != null)
            doc.Author = Author.DeserializeElement(authorElem);
        
        context.AddDoc(doc);

        return doc;
    }

    private static List<IDefinition> ParseDefinitionCollection(IEnumerable<XElement> elements, Doc doc, IDefinition? parent)
    {
        List<IDefinition> defs = new();

        foreach (XElement defElem in elements)
        {
            IDefinition? def = null;
            string defElemName = defElem.Name.LocalName;

            if (defElemName == nameof(Stanza))
            {
                def = new Stanza(parent);
            }
            else if (defElemName is nameof(Section) or "Translation")
            {
                Section section = new(parent);

                string? title = defElem.Attribute(nameof(section.Title))?.Value;
                if (title != null)
                    section.SetTitle(title);

                def = section;
            }
            else if (defElemName == nameof(Run))
            {
                def = new Run(defElem.Value, parent);
            }
            else if (defElemName == nameof(Comment))
            {
                def = new Comment(parent);

                var commentTypeStr = defElem.Attribute("Type")?.Value;
                if (commentTypeStr is not null && Enum.TryParse(commentTypeStr, out CommentType commentType))
                    ((Comment) def).Type = commentType;
            }
            else if (defElemName == "String")
            {
                def = new SimpleContent(null, parent);
            }
            else if (defElemName == "Script")
            {
                DotNetDefinitionScript script = new(defElem.Value);
                def = script;
            }
            else if (defElemName == nameof(Variable))
            {
                var configurableStr = defElem.Attribute("Configurable")?.Value;
                Variable variable = new()
                {
                    Label = defElem.Attribute(nameof(variable.Label))?.Value,
                    DefaultValue = defElem.Attribute(nameof(variable.DefaultValue))?.Value,
                    Configurable = configurableStr is not null && bool.Parse(configurableStr),
                };
                def = variable;
            }
            else if (defElemName == "Translations")
            {
                def = new TranslationCollection(parent: parent);
            }

            if (def == null)
                continue;

            ParseCommonXml(ref def, defElem, doc, def.Parent);

            if (def.Key != null)
                doc.AddDefinition(def);
            defs.Add(def);
        }

        return defs;
    }

    private static void ParseCommonXml(ref IDefinition def, XElement elem, Doc doc, IDefinition? parent)
    {
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
            
        if (def is IContentCollectionContainer contentCollection and IDefinition defCC)
        {
            // Parse elements, remove anything not a ContentPart
            var defColl = ParseDefinitionCollection(elem.Elements(), doc, defCC)
                .OfType<ContentPart>();
            contentCollection.Children.AddRange(defColl);

            var sourceText = elem.Attribute(nameof(contentCollection.Source))?.Value;
            if (sourceText != null)
                contentCollection.Source = new SimpleContent(sourceText, defCC);
        }

        if (def is TranslationCollection translationCollection and IDefinition defTC)
        {
            var translations = ParseDefinitionCollection(elem.Elements(), doc, defTC)
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
    }
}