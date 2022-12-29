using CoptLib.Extensions;
using CoptLib.Models;
using CoptLib.Models.Text;
using CoptLib.Writing;

namespace CoptLib.Scripting.Commands
{
    public class IpaTranscribeCmd : TextCommandBase
    {
        public IpaTranscribeCmd(string name, Run run, IDefinition[] parameters)
            : base(name, run, parameters)
        {
            Parse(name, run, parameters);
        }

        private void Parse(string cmd, Run run, params IDefinition[] parameters)
        {
            Output = parameters[0].Select(Transcribe);
        }

        private void Transcribe(IDefinition def)
        {
            if (def is Run run)
                run.Text = CopticInterpreter.IpaTranscribe(run.Text);
        }
    }
}
