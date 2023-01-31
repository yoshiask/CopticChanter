using CoptLib.Models;
using System.Xml.Serialization;

namespace CoptLib.Scripting;

public class CScript : Definition, ICommandOutput<object>
{
    [XmlText]
    public string ScriptBody { get; set; }

    public object Output { get; protected set; }

    public void Run()
    {
        Output = ScriptingEngine.RunScript(ScriptBody);
        if (Output is IDefinition def)
        {
            def.DocContext = DocContext;
            def.Parent = this;
        }
    }
}
