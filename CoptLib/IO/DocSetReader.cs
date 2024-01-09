using CommunityToolkit.Diagnostics;
using CoptLib.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using OwlCore.Storage;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CoptLib.IO;

public class DocSetReader
{
    private IFolder RootFolder { get; }

    private ILoadContext Context { get; }

    public Dictionary<string, string>? Index { get; private set; }

    public DocSet? Set { get; private set; }

    public DocSetReader(IFolder folder, ILoadContext? context = null)
    {
        Guard.IsNotNull(folder);

        RootFolder = folder;
        Context = context ?? new LoadContext();
    }

    /// <summary>
    /// Reads the set metadata (such as ID and author).
    /// </summary>
    /// <returns></returns>
    [MemberNotNull(nameof(Set))]
    public async Task ReadMetadata()
    {
        if (Set is not null)
            return;
            
        var meta = await RootFolder.GetFirstByNameAsync(DocSetWriter.MetaEntry);
        if (meta is not IFile metaEntry)
            throw new InvalidDataException($"Expected '{meta.Id}' to be a file, got '{meta.GetType()}'");
        if (meta is null)
            throw new InvalidDataException($"Metadata does not exist at '{DocSetWriter.MetaEntry}'");

        using var metaEntryStream = await metaEntry.OpenStreamAsync();
        Set = DocSet.Deserialize(XDocument.Load(metaEntryStream), Context);
    }

    /// <summary>
    /// Reads the set information without parsing any documents.
    /// </summary>
    [MemberNotNull(nameof(Index))]
    public async Task ReadIndex()
    {
        if (Index is not null)
            return;
            
        var index = await RootFolder.GetFirstByNameAsync(DocSetWriter.IndexEntry);
        if (index is not IFile indexEntry)
            throw new InvalidDataException($"Expected '{index.Id}' to be a file, got '{index.GetType()}'");
        if (index is null)
            throw new InvalidDataException($"Index does not exist at '{DocSetWriter.IndexEntry}'");

        using var indexEntryStream = await indexEntry.OpenStreamAsync();
        using StreamReader indexReader = new(indexEntryStream);

        // Read doc list
        Index = new();
        while (!indexReader.EndOfStream)
        {
            string relativePath = await indexReader.ReadLineAsync();
            string name = await indexReader.ReadLineAsync();
            Index.Add(relativePath, name);
        }
    }

    /// <summary>
    /// Reads and parses all <see cref="Doc"/>s in the set.
    /// </summary>
    [MemberNotNull(nameof(Set))]
    [MemberNotNull(nameof(Index))]
    public async Task ReadDocs()
    {
        await ReadMetadata();
        await ReadIndex();

        var docs = await RootFolder.GetFirstByNameAsync(DocSetWriter.DocsDirectory);
        if (docs is not IFolder docsDir)
            throw new InvalidDataException($"Expected '{docs.Id}' to be a folder, got '{docs.GetType()}'");
        if (docs is null)
            throw new InvalidDataException($"Docs directory does not exist at '{DocSetWriter.DocsDirectory}'");

        foreach (string relativePath in Index.Keys)
        {
            // Open entry for doc
            var docItem = await docsDir.GetItemByRelativePathAsync(relativePath);
            if (docItem is not IFile docFile)
                throw new InvalidDataException($"Expected '{relativePath}' to be a file, got '{docItem.GetType()}'");

            // Read XML
            var doc = await Set.Context.LoadDoc(docFile);

            // Add to Set
            Set.IncludedDocs.Add(doc);
        }
    }
}