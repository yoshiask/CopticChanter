using CoptLib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoptLib.Scripting.Commands
{
    public class DefinitionCmd : TextCommandBase
    {
        public DefinitionCmd(string name, Doc context, int startIndex, string[] parameters)
            : base(name, context, startIndex, parameters)
        {
            //DocContext.Definitions.
        }
    }
}
