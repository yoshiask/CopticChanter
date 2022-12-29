using CoptLib.Writing;

namespace CoptLib.Models
{
    /// <summary>
    /// Represents an object that can have a language and font.
    /// </summary>
    public interface IMultilingual
    {
        /// <summary>
        /// The content language.
        /// </summary>
        public LanguageInfo Language { get; set; }

        /// <summary>
        /// The content font.
        /// </summary>
        public string Font { get; set; }

        /// <summary>
        /// Whether the content font has been handled
        /// by calling <see cref="HandleFont"/>.
        /// </summary>
        public bool Handled { get; }

        /// <summary>
        /// Handles the content font.
        /// </summary>
        public void HandleFont();
    }
}
