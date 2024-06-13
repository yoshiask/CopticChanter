using System;
using System.Collections.Generic;
using DynamicExpresso;

namespace CoptLib.Scripting.Patterns;

public static class PatternExtensions
{
    public static Lambda ToLambda(this IPatternSpecification spec, string expressionText,
        IList<string>? paramNames = null)
    {
        if (paramNames is not null && paramNames.Count < spec.ParameterTypes.Count)
            throw new ArgumentException("All parameter names must be specified.");
        
        var parameters = new Parameter[spec.ParameterTypes.Count];
        for (int i = 0; i < parameters.Length; i++)
        {
            var type = spec.ParameterTypes[i];
            var name = paramNames is null ? $"_{i}" : paramNames[i];
            
            parameters[i] = new(name, type);
        }

        Interpreter interpreter = new();
        return interpreter.Parse(expressionText, parameters);
    }
}