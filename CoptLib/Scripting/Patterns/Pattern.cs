using System;
using System.Collections.Generic;
using CoptLib.Models;
using DynamicExpresso;

namespace CoptLib.Scripting.Patterns;

public class Pattern(string patternBody, PatternSpecification spec) : Definition
{
    public string PatternBody { get; } = patternBody;

    public PatternSpecification Spec { get; } = spec;
}

public class DynamicExpressoPattern(string patternBody, PatternSpecification spec, IList<string>? paramNames = null)
    : Pattern(patternBody, spec)
{
    protected Lambda Lambda { get; } = spec.ToLambda(patternBody, paramNames);
    
    public object Evaluate(params object?[] parameters) => Lambda.Invoke(parameters);
}

public class DynamicExpressoPattern<TDelegate>(string patternBody, IList<string>? paramNames = null)
    : DynamicExpressoPattern(patternBody, PatternSpecification.CreateFromDelegate<TDelegate>(), paramNames)
{
    public TDelegate GetDelegate() => Lambda.Compile<TDelegate>();
}