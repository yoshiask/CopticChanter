using CommunityToolkit.Diagnostics;
using CoptLib.Models;
using System.Collections.Generic;
using System.IO;

namespace CoptLib.IO
{
    public class LoadContext
    {
        private readonly Dictionary<string, IDefinition> _definitions = new();

        public List<Doc> LoadedDocs { get; } = new();

        public IReadOnlyDictionary<string, IDefinition> Definitions => _definitions;

        /// <summary>
        /// Gets the <see cref="IDefinition"/> associated with the given key,
        /// optionally scoped to the provided <see cref="Doc"/>.
        /// </summary>
        /// <param name="key">The key to lookup.</param>
        /// <param name="doc">The document to scope to.</param>
        public IDefinition LookupDefinition(string key, Doc doc = null)
        {
            Guard.IsNotNullOrEmpty(key, nameof(key));

            // Check for scoped entry
            if (doc is not null && _definitions.TryGetValue(BuildScopedKey(key, doc), out IDefinition def))
                return def;

            return _definitions[key];

            // Locate all definitions with the given key
            //var defs = LoadedDocs
            //    .Select(d => d.Definitions.ContainsKey(key)
            //        ? d.Definitions[key] : null)
            //    .PruneNull();

            //// If a document was provided, allow it to override
            //IDefinition def = null;
            //if (doc != null)
            //    def = defs.FirstOrDefault(d => d.DocContext.Uuid == doc.Uuid);

            //def ??= defs.FirstOrDefault();

            //return def;
        }

        /// <summary>
        /// Adds an <see cref="IDefinition"/> to the context, scoped to the
        /// given <see cref="Doc"/> if another definition with the same key exists.
        /// </summary>
        /// <param name="definition">The definition to add.</param>
        /// <param name="doc">
        /// The document to scope to.
        /// Pass <see langword="null"/> to override existing entries.
        /// </param>
        public void AddDefinition(IDefinition definition, Doc doc)
        {
            string key = definition.Key;

            // If key already exists and a doc was specified, scope the def
            if (doc is not null && _definitions.ContainsKey(definition.Key))
                key = BuildScopedKey(key, doc);

            _definitions[key] = definition;
        }

        /// <summary>
        /// Loads a document from a given path path.
        /// </summary>
        /// <param name="path">The path to parse from.</param>
        /// <returns>The document that was parsed.</returns>
        public Doc LoadDoc(string path)
        {
            var doc = DocReader.ReadDocXml(path, this);
            LoadedDocs.Add(doc);
            return doc;
        }

        /// <summary>
        /// Loads a document from a given DocXML stream.
        /// </summary>
        /// <param name="file">The path to parse from.</param>
        /// <returns>The document that was parsed.</returns>
        public Doc LoadDoc(Stream file)
        {
            var doc = DocReader.ReadDocXml(file, this);
            LoadedDocs.Add(doc);
            return doc;
        }

        /// <summary>
        /// Loads a document from a DocXMl string.
        /// </summary>
        /// <param name="xml">The XML source string.</param>
        /// <returns>The document that was parsed.</returns>
        public Doc LoadDocFromXml(string xml)
        {
            var doc = DocReader.ParseDocXml(xml, this);
            LoadedDocs.Add(doc);
            return doc;
        }

        /// <summary>
        /// Loads a document from an XML document tree.
        /// </summary>
        /// <param name="xml">The XML to deserialize.</param>
        /// <returns>The document that was parsed.</returns>
        public Doc LoadDocFromXml(System.Xml.Linq.XDocument xml)
        {
            var doc = DocReader.ParseDocXml(xml, this);
            LoadedDocs.Add(doc);
            return doc;
        }

        private static string BuildScopedKey(string key, Doc doc)
        {
            Guard.IsNotNull(key, nameof(key));
            Guard.IsNotNull(doc, nameof(doc));

            return $"{key};DOC='{doc.Uuid}'";
        }
    }
}
