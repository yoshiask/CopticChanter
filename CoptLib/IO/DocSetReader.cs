using CommunityToolkit.Diagnostics;
using CoptLib.Models;
using System.Collections.Generic;
using System.IO;
using OwlCore.Storage;
using System.Threading.Tasks;
using System.Xml.Linq;
using OwlCore.Storage.Archive;

namespace CoptLib.IO
{
    public class DocSetReader
    {
        private IFolder RootFolder { get; set; }

        public Dictionary<string, string> Index { get; private set; }

        public DocSet Set { get; private set; }

        public DocSetReader(IFolder folder)
        {
            RootFolder = folder;
        }

        /// <summary>
        /// Reads the set metadata (such as ID and author).
        /// </summary>
        /// <returns></returns>
        public async Task ReadMetadata()
        {
            var meta = await RootFolder.GetItemAsync(RootFolder.Id + DocSetWriter.META_ENTRY);
            if (meta is not IFile metaEntry)
                throw new InvalidDataException($"Expected '{meta.Id}' to be a file, got '{meta.GetType()}'");
            else if (meta is null)
                throw new InvalidDataException($"Metadata does not exist at '{DocSetWriter.META_ENTRY}'");

            using var metaEntryStream = await metaEntry.OpenStreamAsync();
            Set = DocSet.Deserialize(XDocument.Load(metaEntryStream));
        }

        /// <summary>
        /// Reads the set information without parsing any documents.
        /// </summary>
        public async Task ReadIndex()
        {
            var index = await RootFolder.GetItemAsync(RootFolder.Id + DocSetWriter.INDEX_ENTRY);
            if (index is not IFile indexEntry)
                throw new InvalidDataException($"Expected '{index.Id}' to be a file, got '{index.GetType()}'");
            else if (index is null)
                throw new InvalidDataException($"Index does not exist at '{DocSetWriter.INDEX_ENTRY}'");

            using var indexEntryStream = await indexEntry.OpenStreamAsync();
            using StreamReader indexReader = new(indexEntryStream);

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
        /// Must be called after <see cref="ReadMetadata"/> and <see cref="ReadIndex"/>.
        /// </remarks>
        public async Task ReadDocs()
        {
            Guard.IsNotNull(Index);
            Guard.IsNotNull(RootFolder);

            if (RootFolder is ZipArchiveFolder zipFolder)
            {
                // Workaround for ZipArchiveFolder's crude virtual folder detection
                await zipFolder.CreateFolderAsync(DocSetWriter.DOCS_DIRECTORY);
            }

            var docs = await RootFolder.GetItemAsync($"{RootFolder.Id}{DocSetWriter.DOCS_DIRECTORY}/");
            if (docs is not IFolder docsDir)
                throw new InvalidDataException($"Expected '{docs.Id}' to be a folder, got '{docs.GetType()}'");
            else if (docs is null)
                throw new InvalidDataException($"Docs directory does not exist at '{DocSetWriter.DOCS_DIRECTORY}'");

            foreach (string uuid in Index.Keys)
            {
                // Open entry for doc
                var entry = await docsDir.GetItemAsync(docsDir.Id + uuid);
                if (entry is not IFile entryFile)
                    continue;

                using var docStream = await entryFile.OpenStreamAsync();

                // Read XML
                var doc = Set.Context.LoadDoc(docStream);

                // Add to Set
                Set.IncludedDocs.Add(doc);
            }
        }

        /// <summary>
        /// Reads the set information and meta, then parses all contained documents.
        /// </summary>
        public async Task ReadAll()
        {
            await ReadMetadata();
            await ReadIndex();
            await ReadDocs();
        }
    }
}
