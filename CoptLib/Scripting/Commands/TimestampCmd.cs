using CoptLib.Models;
using CoptLib.Models.Text;
using System;

namespace CoptLib.Scripting.Commands
{
    public class TimestampCmd : TextCommandBase
    {
        public TimestampCmd(string cmd, InlineCommand inline, IDefinition[] parameters)
            : base(cmd, inline, parameters)
        {
            Parse();
        }

        public TimeSpan TimeOffset { get; private set; }

        private void Parse()
        {
            string timePart = (Parameters[0] as IContent)?.SourceText;
            if (!TimeSpan.TryParse(timePart, out var timeOffset))
                return;

            TimeOffset = timeOffset;
        }
    }
}
