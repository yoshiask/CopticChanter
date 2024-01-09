using CoptLib.Models;
using NodaTime;
using OwlCore.Storage;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CoptLib.IO;

public interface ILoadContext
{
    /// <summary>
    /// Gets the current date. Can be overridden using <see cref="SetDate"/>.
    /// </summary>
    LocalDate CurrentDate { get; }

    IReadOnlyDictionary<string, IDefinition> Definitions { get; }

    IReadOnlyList<Doc> LoadedDocs { get; }

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
    void AddDefinition(IDefinition definition, IContextualLoad? contextualItem);

    /// <summary>
    /// Adds the <see cref="Doc"/> and all its <see cref="Doc.DirectDefinitions"/> to this context.
    /// </summary>
    /// <param name="doc">The document to add.</param>
    void AddDoc(Doc doc);

    /// <summary>
    /// Clears the lists of loaded documents and definitions.
    /// </summary>
    void Clear();

    /// <summary>
    /// Loads a document from a given file.
    /// </summary>
    /// <param name="file">The file to parse from.</param>
    /// <returns>The document that was parsed.</returns>
    Task<Doc> LoadDoc(IFile file);

    /// <summary>
    /// Loads a document from a given DocXML stream.
    /// </summary>
    /// <param name="file">The path to parse from.</param>
    /// <returns>The document that was parsed.</returns>
    Doc LoadDoc(Stream file);

    /// <summary>
    /// Loads a document from a given path path.
    /// </summary>
    /// <param name="path">The path to parse from.</param>
    /// <returns>The document that was parsed.</returns>
    Doc LoadDoc(string path);

    /// <summary>
    /// Loads a document from a DocXMl string.
    /// </summary>
    /// <param name="xml">The XML source string.</param>
    /// <returns>The document that was parsed.</returns>
    Doc LoadDocFromXml(string xml);

    /// <summary>
    /// Loads a document from an XML document tree.
    /// </summary>
    /// <param name="xml">The XML to deserialize.</param>
    /// <returns>The document that was parsed.</returns>
    Doc LoadDocFromXml(XDocument xml);

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
    IDefinition? LookupDefinition(string key, IContextualLoad? contextualItem = null);
    
    /// <summary>
    /// Overrides <see cref="CurrentDate"/> to the given time, taking into
    /// account the <see cref="CopticCalendar.TransitionTime"/>.
    /// <para>Set to <see langword="null"/> to use current date and time.</para>
    /// </summary>
    /// <param name="nowOverride">
    /// The date and time to override with.
    /// </param>
    void SetDate(LocalDateTime? nowOverride);

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
    bool TryLookupDefinition(string key, [NotNullWhen(true)] out IDefinition? def, IContextualLoad? contextualItem = null);
}