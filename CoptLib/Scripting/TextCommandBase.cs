using CoptLib.Extensions;
using CoptLib.IO;
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

    public IDefinition? Output { get; set; }

    public bool Evaluated { get; protected set; }

    public IDefinition Execute(ILoadContext? context)
    {
        if (Evaluated)
            return;

        ExecuteInternal(context ?? Inline.DocContext?.Context);
        Evaluated = true;
    }
    
    protected abstract void ExecuteInternal(ILoadContext? context);

    /// <summary>
    /// Ensures that all necessary transforms are applied to the <see cref="Output"/>.
    /// </summary>
    /// <remarks>
    /// Currently, all this method does is drill down to the appropriate translation
    /// if <see cref="Output"/> is an <see cref="ITranslationLookup{IMultilingual}"/>.
    /// </remarks>
    protected void ApplyNecessaryTransforms()
    {
        if (Output is ITranslationLookup<IMultilingual> defLookup)
        {
            var lang = Inline.GetLanguage().Known;
            Output = (IDefinition)defLookup.GetByLanguage(lang);
        }
    }
}