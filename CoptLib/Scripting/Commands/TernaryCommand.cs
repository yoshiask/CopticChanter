using System;
using CoptLib.IO;
using CoptLib.Models;
using CoptLib.Models.Text;

namespace CoptLib.Scripting.Commands;

public class TernaryCommand : TextCommandBase
{
    public TernaryCommand(string name, InlineCommand inline, IDefinition[] parameters) : base(name, inline, parameters)
    {
        if (parameters.Length != 3)
            throw new ArgumentException($"Expected 3 parameters, got {parameters.Length}", nameof(parameters));

        var conditionParameter = parameters[0];
        Condition = conditionParameter switch
        {
            BooleanDefinition boolDef => boolDef.Value,
            ICommandOutput<bool> boolCmd => boolCmd.Output,
            ICommandOutput<object> objCmd => Convert.ToBoolean(objCmd.Output),
            _ => Convert.ToBoolean(conditionParameter)
        };

        TrueResult = parameters[1];
        FalseResult = parameters[2];
    }
    
    public bool Condition { get; }
    
    public IDefinition? TrueResult { get; }
    
    public IDefinition? FalseResult { get; }

    protected override IDefinition? ExecuteInternal(ILoadContext? context) => Condition ? TrueResult : FalseResult;
}