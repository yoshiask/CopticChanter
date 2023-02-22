using CoptLib.Scripting;
using System.Collections.Generic;

namespace CoptLib.Models;

public interface ISupportsTextCommands
{
    /// <summary>
    /// Whether text commands in the content have been
    /// parsed and executed.
    /// </summary>
    bool CommandsHandled { get; set; }

    /// <summary>
    /// A list of commands parsed from the content.
    /// </summary>
    /// <remarks>
    /// This property is populated by calling <see cref="HandleCommands"/>.
    /// </remarks>
    List<TextCommandBase> Commands { get; set; }

    /// <summary>
    /// Parses the content for commands.
    /// </summary>
    void HandleCommands();
}
