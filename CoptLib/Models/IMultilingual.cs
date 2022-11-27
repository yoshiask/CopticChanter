using CoptLib.Writing;

namespace CoptLib.Models
{
    public interface IMultilingual
    {
        public KnownLanguage Language { get; set; }

        public string Font { get; set; }

        public bool Handled { get; }

        public void HandleFont();
    }
}
