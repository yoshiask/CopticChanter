using CoptLib.Models;
using OwlCore.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CoptLib.IO
{
    public class DocSetWriter
    {
        internal static string DOCS_DIRECTORY = "docs";
        internal static string INDEX_ENTRY = "index.tsv";
        internal static string META_ENTRY = "meta.xml";

        public DocSet Set { get; }

        public DocSetWriter(DocSet set)
        {
            Set = set;
        }

        public DocSetWriter(string uuid, string name, IEnumerable<Doc> docs = null)
        {
            Set = new(uuid, name, docs);
        }

        public async Task Write(IModifiableFolder rootFolder)
        {
            // Begin building index
            StringBuilder sb = new();

            var docs = await rootFolder.CreateFolderAsync(DOCS_DIRECTORY);
            if (docs is not IModifiableFolder docsDir)
                throw new InvalidOperationException($"'{rootFolder.Id}' and all its subfolders must be modifiable.");

            foreach (var doc in Set.IncludedDocs)
            {
                // Write each Document to its own entry
                var docEntry = await docsDir.CreateFileAsync(doc.Uuid);
                using var docEntryStream = await docEntry.OpenStreamAsync(FileAccess.Write);
                DocWriter.WriteDocXml(doc, docEntryStream);

                // Write the Document ID and name to index
                sb.AppendLine(doc.Uuid + "\t" + doc.Name);
            }

            // Write the index to an entry
            var indexEntry = await rootFolder.CreateFileAsync(INDEX_ENTRY);
            using (var indexEntryStream = await indexEntry.OpenStreamAsync(FileAccess.Write))
            using (StreamWriter sw = new(indexEntryStream))
            {
                sw.Write(sb.ToString());
            }

            // Write additional metadata
            var metaEntry = await rootFolder.CreateFileAsync(META_ENTRY);
            using (var metaEntryStream = await metaEntry.OpenStreamAsync(FileAccess.Write))
            using (XmlTextWriter xw = new(metaEntryStream, Encoding.Unicode))
            {
                var xml = Set.Serialize();
                xml.WriteTo(xw);
            }
        }
    }
}
