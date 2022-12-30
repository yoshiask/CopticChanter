using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;

namespace CoptLib.Scripting.Commands
{
    public class IpaTranscribeCmd : TextCommandBase
    {
        public IpaTranscribeCmd(string name, InlineCommand inline, IDefinition[] parameters)
            : base(name, inline, parameters)
        {
            Parse();
        }

        private void Parse()
        {
            Output = Parameters[0].Select(Transcribe);
        }

        private void Transcribe(IDefinition def)
        {
            if (def is Run run)
                run.Text = CopticInterpreter.IpaTranscribe(run.Text);
        }
    }
}
