using CoptLib.Models;
using CoptLib.Scripting;
using CoptLib.Writing;
using OwlCore.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Xml.Linq;
using System.Xml.Serialization;

#if DEBUG
using Output = System.Diagnostics.Debug;
#else
using Output = System.Console;
#endif

namespace CoptLib.IO
{
    public static class DocReader
    {
        public static IDictionary<string, Doc> AllDocs = new Dictionary<string, Doc>();

        /// <summary>
        /// Deserializes and returns a DocXML file. Only use in full .NET Framework
        /// </summary>
        /// <param name="path">The path to the XML file</param>
        /// <returns></returns>
        public static Doc ReadDocXml(string path)
        {
            try
            {
                return ParseDocXml(XDocument.Load(path));
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
        public static Doc ReadDocXml(Stream file)
        {
            try
            {
                return ParseDocXml(XDocument.Load(file, LoadOptions.None));
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
        public static Doc ParseDocXml(string xml) => ParseDocXml(XDocument.Parse(xml));

        /// <summary>
        /// Parses the XML document tree into a <see cref="Doc"/>.
        /// </summary>
        public static Doc ParseDocXml(XDocument xml)
        {
            // The actual content can't be directly deserialized, so it needs to be manually parsed
            Doc doc = new()
            {
                Name = xml.Root.Element("Name")?.Value,
                Uuid = xml.Root.Element("Uuid")?.Value,
            };

            var defsElem = xml.Root.Element("Definitions");
            if (defsElem != null)
            {
                var defs = ParseDefinitionCollection(defsElem?.Elements(), doc, null);
                doc.DirectDefinitions = defs;
            }

            var transsElem = xml.Root.Element("Translations");
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

        /// <summary>
        /// Parses text commands and applies font conversions in the given document.
        /// </summary>
        /// <param name="doc">The document to apply all transforms on.</param>
        public static void ApplyDocTransforms(Doc doc)
        {
            RecursiveTransform(doc.DirectDefinitions);
            RecursiveTransform(doc.Translations.Children);
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
                else if (defElemName == nameof(Section) || defElemName == "Translation")
                {
                    Section section = new(parent);

                    string title = defElem.Attribute("Title")?.Value;
                    if (title != null)
                        section.Title = new Stanza(section)
                        {
                            SourceText = title,
                        };

                    def = section;
                }
                else if (defElemName == nameof(Script))
                {
                    Script script = new()
                    {
                        LuaScript = defElem.Value
                    };
                    def = script;
                }
                else if (defElemName == nameof(Variable))
                {
                    Variable variable = new()
                    {
                        Label = defElem.Attribute("Label")?.Value,
                        DefaultValue = defElem.Attribute("DefaultValue")?.Value,
                        Configurable = bool.Parse(defElem.Attribute("Configurable")?.Value),
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
                def.Key = elem.Attribute("Key")?.Value;

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
                multilingual.Font = elem.Attribute("Font")?.Value;

                string langVal = elem.Attribute("Language")?.Value;
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
                    .Select(d => d as ContentPart)
                    .PruneNull();
                contentCollection.Children.AddRange(defColl);

                string sourceText = elem.Attribute("Source")?.Value;
                if (sourceText != null)
                    contentCollection.Source = new SimpleContent(sourceText, defCC);
            }
        }

        internal static void RecursiveTransform(System.Collections.IEnumerable parts)
        {
            foreach (var part in parts)
                Transform(part);
        }

        internal static void Transform(object part)
        {
            if (part is Script partScript)
                partScript.Run();

            if (part is ICommandOutput partCmdOut && partCmdOut.Output != null)
                part = partCmdOut.Output;

            if (part is IContent partContent)
            {
                if (part is IMultilingual partMulti && partMulti.Language?.Known == KnownLanguage.Coptic)
                    partContent.SourceText = CopticInterpreter.ExpandAbbreviations(partContent.SourceText);

                partContent.ParseCommands();
            }

            if (part is IContentCollectionContainer partCollection)
            {
                if (partCollection.Source != null)
                {
                    // Populate the collection with items from the source.
                    // This is done before commands are parsed, just in
                    // case the generated content contains commands.
                    partCollection.Source.ParseCommands();
                    var cmd = partCollection.Source.Commands.LastOrDefault();

                    if (cmd.Output != null)
                    {
                        bool hasExplicitChildren = partCollection.Children.Count > 0;

                        if (cmd.Output is IContentCollectionContainer cmdOutCollection)
                            partCollection.Children.AddRange(cmdOutCollection.Children);
                        else if (cmd.Output is ContentPart cmdOutPart)
                            partCollection.Children.Add(cmdOutPart);

                        // If the collection doesn't have any explicit elements (in other words,
                        // it's only children came from the source), inherit the language of the children
                        if (!hasExplicitChildren && part is IMultilingual partMulti)
                        {
                            partMulti.Language = (cmd.Output as IMultilingual)?.Language;
                            partMulti.Font = (cmd.Output as IMultilingual)?.Font;
                        }
                    }
                }

                partCollection.ParseCommands();
            }

            if (part is IMultilingual multilingual)
                multilingual.HandleFont();

            if (part is IDefinition def && def.Key != null && !def.DocContext.Definitions.ContainsKey(def.Key))
                def.DocContext.AddDefinition(def);
        }
    }
}
