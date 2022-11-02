using System.Collections.Generic;

namespace CoptLib.Models
{
    /// <summary>
    /// Interface for any model that contains a collection of <see cref="ContentPart"/>s.
    /// </summary>
    public interface IContentCollectionContainer
    {
        List<ContentPart> Content { get; set; }

        /// <summary>
        /// Returns the number of rows this section requires to display
        /// all section headers and stanzas
        /// </summary>
        int CountRows();
    }
}
