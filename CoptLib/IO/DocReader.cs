using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Scripting;
using CoptLib.Writing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

#if DEBUG
using Output = System.Diagnostics.Debug;
#else
using Output = System.Console;
#endif

namespace CoptLib.IO
{
    public static class DocReader
    {
        /// <summary>
        /// Deserializes and returns a DocXML file. Only use in full .NET Framework
        /// </summary>
        /// <param name="path">The path to the XML file</param>
        /// <returns></returns>
        public static Doc ReadDocXml(string path, LoadContextBase context = null)
        {
            try
            {
                return ParseDocXml(XDocument.Load(path), context);
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Deserializes and returns a DocXML file. For use in UWP
        /// </summary>
        /// <param name="file">A Stream of the XML file</param>
        /// <returns></returns>
        public static Doc ReadDocXml(Stream file, LoadContextBase context = null)
        {
            try
            {
                return ParseDocXml(XDocument.Load(file, LoadOptions.None), context);
            }
            catch (Exception ex)
            {
                Output.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Parses the XML string into a <see cref="Doc"/>.
        /// </summary>
        public static Doc ParseDocXml(string xml, LoadContextBase context = null) => ParseDocXml(XDocument.Parse(xml), context);

        /// <summary>
        /// Parses the XML document tree into a <see cref="Doc"/>.
        /// </summary>
        public static Doc ParseDocXml(XDocument xml, LoadContextBase context = null)
        {
            // The actual content can't be directly deserialized, so it needs to be manually parsed
            Doc doc = new(context ?? new LoadContext())
            {
                Name = xml.Root.Element(nameof(doc.Name))?.Value,
                Key = xml.Root.Element(nameof(doc.Key))?.Value,
            };

            // BACKCOMPAT: Support documents that use the Uuid element name
            doc.Key ??= xml.Root.Element("Uuid")?.Value;

            var defsElem = xml.Root.Element("Definitions");
            if (defsElem != null)
            {
                var defs = ParseDefinitionCollection(defsElem?.Elements(), doc, null);
                doc.DirectDefinitions = defs;
            }

            var transsElem = xml.Root.Element(nameof(doc.Translations));
            if (transsElem != null)
            {
                foreach (var def in ParseDefinitionCollection(transsElem.Elements(), doc, null))
                {
                    if (def is not ContentPart translation)
                        continue;
                    doc.Translations.Children.Add(translation);
                }
            }

            return doc;
        }

        private static List<IDefinition> ParseDefinitionCollection(IEnumerable<XElement> elements, Doc doc, IDefinition parent)
        {
            List<IDefinition> defs = new();

            foreach (XElement defElem in elements)
            {
                IDefinition def = null;
                string defElemName = defElem.Name.LocalName;

                if (defElemName == nameof(Stanza))
                {
                    def = new Stanza(parent);
                }
                else if (defElemName == "String")
                {
                    def = new SimpleContent(null, parent);
                }
                else if (defElemName == nameof(Comment))
                {
                    def = new Comment(parent)
                    {
                        Type = defElem.Attribute("Type")?.Value
                    };
                }
                else if (defElemName == nameof(Section) || defElemName == "Translation")
                {
                    Section section = new(parent);

                    string title = defElem.Attribute(nameof(section.Title))?.Value;
                    if (title != null)
                        section.Title = new Stanza(section)
                        {
                            SourceText = title,
                        };

                    def = section;
                }
                else if (defElemName == "Script")
                {
                    CScript script = new()
                    {
                        ScriptBody = defElem.Value
                    };
                    def = script;
                }
                else if (defElemName == nameof(Variable))
                {
                    Variable variable = new()
                    {
                        Label = defElem.Attribute(nameof(variable.Label))?.Value,
                        DefaultValue = defElem.Attribute(nameof(variable.DefaultValue))?.Value,
                        Configurable = bool.Parse(defElem.Attribute(nameof(variable.Configurable))?.Value),
                    };
                    def = variable;
                }
                else if (defElemName == "Translations")
                {
                    def = new TranslationCollection(parent);
                }

                if (def == null)
                    continue;

                ParseCommonXml(def, defElem, doc, def.Parent);

                if (def.Key != null)
                    doc.AddDefinition(def);
                defs.Add(def);
            }

            return defs;
        }

        private static void ParseCommonXml(object obj, XElement elem, Doc doc, IDefinition parent)
        {
            if (obj is IDefinition def)
            {
                def.DocContext = doc;
                def.Parent = parent;
                def.Key = elem.Attribute(nameof(def.Key))?.Value;

                // Not every IDefinition is explicitly defined,
                // but since this branch only runs when we already
                // have an XML element, this is a safe assumption.
                def.IsExplicitlyDefined = true;
            }
            if (obj is IContent content)
            {
                content.SourceText = elem.Value;
            }
            if (obj is IMultilingual multilingual)
            {
                multilingual.Font = elem.Attribute(nameof(multilingual.Font))?.Value;

                string langVal = elem.Attribute(nameof(multilingual.Language))?.Value;
                if (!string.IsNullOrEmpty(langVal))
                {
                    multilingual.Language = LanguageInfo.Parse(langVal);
                }

                if (parent is IMultilingual parentMultilingual)
                {
                    multilingual.Font ??= parentMultilingual.Font;
                    multilingual.Language ??= parentMultilingual.Language;
                }

                if (obj is Section sectionMulti && sectionMulti.Title is IMultilingual sectionTitleMulti)
                {
                    sectionTitleMulti.Font ??= sectionMulti.Font;
                    sectionTitleMulti.Language ??= sectionMulti.Language;
                }
            }
            if (obj is IContentCollectionContainer contentCollection && obj is IDefinition defCC)
            {
                // Parse elements, remove anything not a ContentPart
                var defColl = ParseDefinitionCollection(elem.Elements(), doc, defCC)
                    .ElementsAs<ContentPart>();
                contentCollection.Children.AddRange(defColl);

                string sourceText = elem.Attribute(nameof(contentCollection.Source))?.Value;
                if (sourceText != null)
                    contentCollection.Source = new SimpleContent(sourceText, defCC);
            }
        }
    }
}
