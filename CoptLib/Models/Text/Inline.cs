using CoptLib.Writing;

namespace CoptLib.Models.Text
{
    /// <summary>
    /// An abstract class used as the base class for the also-abstract <see cref="Run"/> class.
    /// Inline supports common API for classes involved in the text object model,
    /// such as properties that control language, font families, and so on.
    /// </summary>
    public abstract class Inline : IDefinition, IMultilingual
    {
        public Inline(IDefinition parent)
        {
            Parent = parent;
            DocContext = parent?.DocContext;

            if (Parent is IMultilingual multi)
            {
                Language = multi.Language;
                Font = multi.Font;
            }
        }

        public LanguageInfo Language { get; set; }

        public string Font { get; set; }

        public bool FontHandled { get; protected set; }

        public string Key { get; set; }

        public Doc DocContext { get; set; }

        public IDefinition Parent { get; set; }

        public bool IsExplicitlyDefined { get; set; }

        public abstract void HandleFont();
    }
}
