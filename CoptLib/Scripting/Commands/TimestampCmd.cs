using CoptLib.Models;
using System;

namespace CoptLib.Scripting.Commands
{
    public class TimestampCmd : TextCommandBase
    {
        public TimestampCmd(string cmd, IContent content, Doc context, int startIndex, string[] parameters)
            : base(cmd, content, context, startIndex, parameters)
        {
            Parse(cmd, parameters);
        }

        public TimeSpan TimeOffset { get; private set; }

        private void Parse(string cmd, string[] parameters)
        {
            string timePart = parameters[0];
            if (!TimeSpan.TryParse(timePart, out var timeOffset))
                return;

            TimeOffset = timeOffset;
            Text = string.Empty;
        }
    }
}
