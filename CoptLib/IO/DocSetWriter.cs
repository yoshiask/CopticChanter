using CoptLib.Models;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CoptLib.IO
{
    public class DocSetWriter
    {
        internal static string DOC_ENTRY_PREFIX = $"docs{Path.DirectorySeparatorChar}";
        internal static string INDEX_ENTRY = "index";

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
            sb.AppendLine(Set.Uuid);
            sb.AppendLine(Set.Name);
            sb.AppendLine(Set.Author.ToXmlString());

            foreach (var doc in Set.IncludedDocs)
            {
                // Write each Document to its own entry
                var docEntry = archive.CreateEntry(DOC_ENTRY_PREFIX + doc.Uuid);
                using var docEntryStream = docEntry.Open();
                DocWriter.WriteDocXml(doc, docEntryStream);

                // Write the Document ID and name to index
                sb.AppendLine(doc.Uuid + "\t" + doc.Name);
            }

            // Write the index to an entry
            var indexEntry = archive.CreateEntry(INDEX_ENTRY);
            using var indexEntryStream = indexEntry.Open();
            using StreamWriter sw = new(indexEntryStream);
            sw.Write(sb.ToString());
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
