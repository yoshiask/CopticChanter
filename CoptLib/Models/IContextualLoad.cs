﻿using CoptLib.IO;

namespace CoptLib.Models;

/// <summary>
/// Represents anything that can directly use a <see cref="LoadContext"/>.
/// </summary>
public interface IContextualLoad
{
    /// <summary>
    /// The unique identifier of the current item.
    /// </summary>
    string Uuid { get; set; }

    /// <summary>
    /// The context this item was loaded in.
    /// </summary>
    LoadContext Context { get; set; }
}