using System.Collections.Generic;

namespace CoptLib.Models
{
    /// <summary>
    /// Interface for any model that contains a collection of <see cref="ContentPart"/>s.
    /// </summary>
    public interface IContentCollectionContainer : ICollection<ContentPart>, IDefinition
    {
        /// <summary>
        /// A command used to populate the collection. May contain nested text commands.
        /// </summary>
        SimpleContent Source { get; set; }

        /// <inheritdoc cref="IContent.ParseCommands"/>
        void ParseCommands();
    }
}
