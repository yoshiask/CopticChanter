using System;
using CommunityToolkit.Diagnostics;
using CoptLib.Models;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using OwlCore.Storage;

namespace CoptLib.IO;

public abstract class LoadContextBase
{
    private readonly Dictionary<string, IDefinition> _definitions = new();
    private readonly List<Doc> _loadedDocs = new();

    public IReadOnlyDictionary<string, IDefinition> Definitions => _definitions;

    public IReadOnlyList<Doc> LoadedDocs => _loadedDocs;

    private static string BuildScopedKey(string key, IContextualLoad ctxItem)
    {
        Guard.IsNotNull(key, nameof(key));
        Guard.IsNotNull(ctxItem, nameof(ctxItem));

        return $"{key};Scope='{ctxItem.Key}'";
    }

    /// <summary>
    /// Adds an <see cref="IDefinition"/> to the context, scoped to the
    /// given <see cref="Doc"/> or <see cref="DocSet"/> if another
    /// definition with the same key exists.
    /// </summary>
    /// <param name="definition">The definition to add.</param>
    /// <param name="contextualItem">
    /// The document or set to scope to.
    /// Pass <see langword="null"/> to override existing global entries.
    /// </param>
    public void AddDefinition(IDefinition definition, IContextualLoad? contextualItem)
    {
        var key = definition.Key
                  ?? throw new ArgumentNullException(nameof(definition), "Definition must specify a key.");

        // If key already exists and a ctxItem was specified, scope the def
        if (contextualItem is not null && _definitions.ContainsKey(key))
            key = BuildScopedKey(key, contextualItem);

        _definitions[key] = definition;
    }

    /// <summary>
    /// Clears the lists of loaded documents and definitions.
    /// </summary>
    public void Clear()
    {
        _definitions.Clear();
        _loadedDocs.Clear();
    }

    public void AddDoc(Doc doc)
    {
        if (_loadedDocs.Contains(doc))
            return;

        _loadedDocs.Add(doc);

        AddDefinition(doc, doc);
        foreach (var def in doc.DirectDefinitions)
            AddDefinition(def, doc);
    }

    /// <summary>
    /// Loads a document from a given DocXML stream.
    /// </summary>
    /// <param name="file">The path to parse from.</param>
    /// <returns>The document that was parsed.</returns>
    public Doc LoadDoc(Stream file)
    {
        var doc = DocReader.ReadDocXml(file, this);
        AddDoc(doc);
        return doc;
    }

    /// <summary>
    /// Loads a document from a given file.
    /// </summary>
    /// <param name="file">The file to parse from.</param>
    /// <returns>The document that was parsed.</returns>
    public async Task<Doc> LoadDoc(IFile file)
    {
        using var stream = await file.OpenStreamAsync();
        return LoadDoc(stream);
    }

    /// <summary>
    /// Loads a document from a given path path.
    /// </summary>
    /// <param name="path">The path to parse from.</param>
    /// <returns>The document that was parsed.</returns>
    public Doc LoadDoc(string path)
    {
        var doc = DocReader.ReadDocXml(path, this);
        AddDoc(doc);
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
        AddDoc(doc);
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
        AddDoc(doc);
        return doc;
    }

    /// <summary>
    /// Gets the <see cref="IDefinition"/> associated with the given key,
    /// optionally scoped to the provided <see cref="Doc"/> or <see cref="DocSet"/>.
    /// </summary>
    /// <param name="key">The key to lookup.</param>
    /// <param name="contextualItem">
    /// The document or set to scope to.
    /// Pass <see langword="null"/> to search only global entries.
    /// </param>
    /// <returns>
    /// An <see cref="IDefinition"/> with the given key, or <see langword="null"/> if none was found.
    /// </returns>
    public virtual IDefinition? LookupDefinition(string key, IContextualLoad? contextualItem = null)
    {
        Guard.IsNotNullOrEmpty(key, nameof(key));

        // Check for scoped entry
        if (contextualItem is not null && _definitions.TryGetValue(BuildScopedKey(key, contextualItem), out var def))
            return def;

        return _definitions.TryGetValue(key, out def) ? def : null;
    }

    /// <summary>
    /// Tries to get the <see cref="IDefinition"/> associated with the given key,
    /// optionally scoped to the provided <see cref="Doc"/> or <see cref="DocSet"/>.
    /// </summary>
    /// <param name="key">The key to lookup.</param>
    /// <param name="contextualItem">
    /// The document or set to scope to.
    /// Pass <see langword="null"/> to search only global entries.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if a definition was found,
    /// <see langword="false"/> if not.
    /// </returns>
    public bool TryLookupDefinition(string key, [NotNullWhen(true)] out IDefinition? def, IContextualLoad? contextualItem = null)
    {
        def = LookupDefinition(key);
        return def is not null;
    }
}