using CoptLib.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
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

        public void Write(Stream stream)
        {
            using ZipArchive archive = new(stream, ZipArchiveMode.Create);

            // Begin building index
            StringBuilder sb = new();

            foreach (var doc in Set.IncludedDocs)
            {
                // Write each Document to its own entry
                var docEntry = archive.CreateEntry(Path.Combine(DOCS_DIRECTORY, doc.Uuid));
                using var docEntryStream = docEntry.Open();
                DocWriter.WriteDocXml(doc, docEntryStream);

                // Write the Document ID and name to index
                sb.AppendLine(doc.Uuid + "\t" + doc.Name);
            }

            // Write the index to an entry
            var indexEntry = archive.CreateEntry(INDEX_ENTRY);
            using (var indexEntryStream = indexEntry.Open())
            using (StreamWriter sw = new(indexEntryStream))
            {
                sw.Write(sb.ToString());
            }

            // Write additional metadata
            var metaEntry = archive.CreateEntry(META_ENTRY);
            using (var metaEntryStream = metaEntry.Open())
            using (XmlTextWriter xw = new(metaEntryStream, Encoding.Unicode))
            {
                var xml = Set.Serialize();
                xml.WriteTo(xw);
            }
        }

        /// <summary>
        /// Saves the set to a file.
        /// </summary>
        public void Write(string path)
        {
            using var fileStream = File.Open(path, FileMode.OpenOrCreate);
            Write(fileStream);
        }
    }
}
