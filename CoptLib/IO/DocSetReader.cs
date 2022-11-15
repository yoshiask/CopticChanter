using CommunityToolkit.Diagnostics;
using CoptLib.Models;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Xml.Serialization;

namespace CoptLib.IO
{
    public class DocSetReader : IDisposable
    {
        private ZipArchive Archive { get; set; }

        public Dictionary<string, string> Index { get; private set; }

        public DocSet Set { get; private set; }

        public DocSetReader(Stream zipStream)
        {
            Archive = new(zipStream);
        }

        public DocSetReader(string filePath) : this(File.OpenRead(filePath))
        {

        }

        /// <summary>
        /// Reads the set information without parsing any documents.
        /// </summary>
        public void ReadIndex()
        {
            var indexEntry = Archive.GetEntry(DocSetWriter.INDEX_ENTRY);
            using var indexEntryStream = indexEntry.Open();
            using StreamReader indexReader = new(indexEntryStream);

            // Create empty set object
            Set = new(indexReader.ReadLine(), indexReader.ReadLine());

            // Deserialize Author XML
            string authorXml = indexReader.ReadLine();
            XmlSerializer serializer = new(typeof(Author));
            Set.Author = (Author)serializer.Deserialize(new StringReader(authorXml));

            // Read doc list
            Index = new();
            while (!indexReader.EndOfStream)
            {
                string[] parts = indexReader.ReadLine().Split('\t');
                Index.Add(parts[0], parts[1]);
            }
        }

        /// <summary>
        /// Reads and parses all <see cref="Doc"/>s in the set.
        /// </summary>
        /// <remarks>
        /// Must be called after <see cref="ReadIndex"/>.
        /// </remarks>
        public void ReadDocs()
        {
            Guard.IsNotNull(Index);
            Guard.IsNotNull(Archive);

            foreach (string uuid in Index.Keys)
            {
                // Open entry for doc
                var entry = Archive.GetEntry(DocSetWriter.DOC_ENTRY_PREFIX + uuid);
                using var docStream = entry.Open();

                // Read XML
                var doc = DocReader.ReadDocXml(docStream);

                // Add to Set
                Set.IncludedDocs.Add(doc);
            }
        }

        /// <summary>
        /// Reads the set information and index, then parses all contained documents.
        /// </summary>
        public void ReadAll()
        {
            ReadIndex();
            ReadDocs();
        }

        public void Dispose() => Archive.Dispose();
    }
}
