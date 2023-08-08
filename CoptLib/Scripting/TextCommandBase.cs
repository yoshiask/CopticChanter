using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;

namespace CoptLib.Scripting;

/// <summary>
/// Represents a command that was embedded in a <see cref="Run"/>.
/// </summary>
public abstract class TextCommandBase : ICommandOutput<IDefinition>
{
    public TextCommandBase(string name, InlineCommand inline, IDefinition[] parameters)
    {
        Name = name;
        Inline = inline;
        Parameters = parameters;
    }

    /// <summary>
    /// The name of the command.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The parameters used to call the command.
    /// </summary>
    public IDefinition[] Parameters { get; }

    /// <summary>
    /// The content that contained the command.
    /// </summary>
    public InlineCommand Inline { get; }

    public Doc? DocContext => Inline.DocContext;

    public IDefinition? Output { get; set; }

    public bool Evaluated { get; protected set; }

    protected void HandleOutput()
    {
        if (Output is ITranslationLookup<IMultilingual> defLookup)
        {
            var lang = Inline.GetLanguage().Known;
            Output = (IDefinition)defLookup.GetByLanguage(lang);
        }
    }
}