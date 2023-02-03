using CoptLib.IO;

namespace CoptLib.Models;

/// <summary>
/// Represents anything that can directly use a <see cref="LoadContext"/>.
/// </summary>
public interface IContextualLoad
{
    /// <summary>
    /// The unique identifier of the current item.
    /// </summary>
    string Key { get; set; }

    /// <summary>
    /// The context this item was loaded in.
    /// </summary>
    LoadContextBase Context { get; set; }
}
