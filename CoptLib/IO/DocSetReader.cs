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
        private IFolder RootFolder { get; }

        private LoadContextBase Context { get; }

        public Dictionary<string, string> Index { get; private set; }

        public DocSet Set { get; private set; }

        public DocSetReader(IFolder folder, LoadContextBase context = null)
        {
            Guard.IsNotNull(folder);

            RootFolder = folder;
            Context = context ?? new();
        }

        /// <summary>
        /// Reads the set metadata (such as ID and author).
        /// </summary>
        /// <returns></returns>
        public async Task ReadMetadata()
        {
            var meta = await RootFolder.GetFirstByNameAsync(DocSetWriter.META_ENTRY);
            if (meta is not IFile metaEntry)
                throw new InvalidDataException($"Expected '{meta.Id}' to be a file, got '{meta.GetType()}'");
            else if (meta is null)
                throw new InvalidDataException($"Metadata does not exist at '{DocSetWriter.META_ENTRY}'");

            using var metaEntryStream = await metaEntry.OpenStreamAsync();
            Set = DocSet.Deserialize(XDocument.Load(metaEntryStream), Context);
        }

        /// <summary>
        /// Reads the set information without parsing any documents.
        /// </summary>
        public async Task ReadIndex()
        {
            var index = await RootFolder.GetFirstByNameAsync(DocSetWriter.INDEX_ENTRY);
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
                string relativePath = indexReader.ReadLine();
                string name = indexReader.ReadLine();
                Index.Add(relativePath, name);
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

            if (RootFolder is ZipArchiveFolder zipFolder)
            {
                // Workaround for ZipArchiveFolder's crude virtual folder detection
                await zipFolder.CreateFolderAsync(DocSetWriter.DOCS_DIRECTORY);
            }

            var docs = await RootFolder.GetFirstByNameAsync(DocSetWriter.DOCS_DIRECTORY);
            if (docs is not IFolder docsDir)
                throw new InvalidDataException($"Expected '{docs.Id}' to be a folder, got '{docs.GetType()}'");
            else if (docs is null)
                throw new InvalidDataException($"Docs directory does not exist at '{DocSetWriter.DOCS_DIRECTORY}'");

            foreach (string relativePath in Index.Keys)
            {
                // Open entry for doc
                var docItem = await docsDir.GetItemByRelativePathAsync(relativePath);
                if (docItem is not IFile docFile)
                    throw new InvalidDataException($"Expected '{relativePath}' to be a file, got '{docItem.GetType()}'");

                using var docFileStream = await docFile.OpenStreamAsync();

                // Read XML
                var doc = Set.Context.LoadDoc(docFileStream);

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
