using System.Collections.Generic;

namespace CoptLib.Models
{
    /// <summary>
    /// Interface for any model that contains a collection of <see cref="ContentPart"/>s.
    /// </summary>
    public interface IContentCollectionContainer : ICollection<ContentPart>
    {
        /// <summary>
        /// The ID of a <see cref="Definition"/> to use to populate <see cref="Content"/>.
        /// </summary>
        string Source { get; set; }

        /// <inheritdoc cref="IContent.ParseCommands"/>
        void ParseCommands();
    }
}
