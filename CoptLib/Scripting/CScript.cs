using CoptLib.Models;
using System.Xml.Serialization;

namespace CoptLib.Scripting;

public class CScript : Definition, ICommandOutput<IDefinition>
{
    [XmlText]
    public string ScriptBody { get; set; }

    public IDefinition Output { get; protected set; }

    public bool Evaluated { get; protected set; }

    public void Run()
    {
        if (Evaluated)
            return;

        Output = ScriptingEngine.RunScript(ScriptBody);
        Output.DocContext = DocContext;
        Output.Parent = this;

        Evaluated = true;
    }
}
