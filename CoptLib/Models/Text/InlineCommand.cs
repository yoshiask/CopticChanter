using CommunityToolkit.Diagnostics;
using CoptLib.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoptLib.Models.Text
{
    internal class InlineCommand : Inline
    {
        public InlineCommand(string cmdName, IDefinition parent) : base(parent)
        {
            CommandName = cmdName;
        }

        public InlineCommand(ReadOnlySpan<char> cmdName, IDefinition parent) : this(new string(cmdName.ToArray()), parent)
        {
        }

        public string CommandName { get; set; }

        public TextCommandBase Command { get; set; }

        public InlineCollection Parameters { get; set; }

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
            return $"\\{CommandName}{{{string.Join("|", Parameters.Select(i => i.ToString()))}}}";
        }
    }
}
