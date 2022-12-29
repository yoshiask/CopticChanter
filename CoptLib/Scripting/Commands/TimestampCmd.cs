using CoptLib.Models;
using CoptLib.Models.Text;
using System;

namespace CoptLib.Scripting.Commands
{
    public class TimestampCmd : TextCommandBase
    {
        public TimestampCmd(string cmd, Run run, IDefinition[] parameters)
            : base(cmd, run, parameters)
        {
            Parse(cmd, parameters);
        }

        public TimeSpan TimeOffset { get; private set; }

        private void Parse(string cmd, IDefinition[] parameters)
        {
            string timePart = ((IContent)parameters[0]).SourceText;
            if (!TimeSpan.TryParse(timePart, out var timeOffset))
                return;

            TimeOffset = timeOffset;
        }
    }
}
