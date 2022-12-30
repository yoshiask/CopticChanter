using CommunityToolkit.Diagnostics;
using CoptLib.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoptLib.Models.Text
{
    public class InlineCommand : Inline
    {
        public InlineCommand(string cmdName, IDefinition parent) : base(parent)
        {
            CommandName = cmdName;
        }

        public InlineCommand(ReadOnlySpan<char> cmdName, IDefinition parent) : this(new string(cmdName.ToArray()), parent)
        {
        }

        /// <summary>
        /// The name of the command.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// The <see cref="Inline"/>s passed as parameters to the command.
        /// </summary>
        public InlineCollection Parameters { get; set; }

        /// <summary>
        /// The <see cref="TextCommandBase"/> that was run.
        /// </summary>
        public TextCommandBase Command { get; set; }

        public override void HandleFont()
        {
            if (Handled)
                return;

            Guard.IsNotNull(Parameters);

            foreach (Inline inline in Parameters)
                inline.HandleFont();

            Handled = true;
        }

        public override string ToString()
        {
            if (Command?.Output == null)
                return $"\\{CommandName}{{{string.Join("|", Parameters.Select(i => i.ToString()))}}}";
            else
                return Command.Output.ToString();
        }
    }
}
