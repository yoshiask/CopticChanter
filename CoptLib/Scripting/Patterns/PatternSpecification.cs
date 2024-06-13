using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CoptLib.Scripting.Patterns;

public interface IPatternSpecification
{
    IReadOnlyList<Type> ParameterTypes { get; }
    Type ReturnType { get; }
}

public class PatternSpecification(IReadOnlyList<Type> parameterTypes, Type returnType) : IPatternSpecification
{
    public IReadOnlyList<Type> ParameterTypes { get; } = parameterTypes;
    public Type ReturnType { get; } = returnType;

    public override string ToString() =>
        $"{string.Join(", ", ParameterTypes.Select(t => t.Name))} > {ReturnType.Name}";

    public static PatternSpecification CreateFromDelegate<TDelegate>()
    {
        var delegateType = typeof(TDelegate);
        if (!typeof(Delegate).IsAssignableFrom(delegateType))
            throw new ArgumentException($"{nameof(delegateType)} must be a delegate type.");

        var invoke = delegateType.GetMethods().FirstOrDefault(m => m.Name == "Invoke");
        if (invoke is null)
            throw new Exception($"Type '{delegateType}' does not have an Invoke method.");
    
        var parameterTypes = invoke.GetParameters().Select(p => p.ParameterType).ToList();
        var returnType = invoke.ReturnType;
    
        return new(parameterTypes, returnType);
    }
}

public class StringPatternSpecification : PatternSpecification
{
    private StringPatternSpecification(string specification, IReadOnlyList<Type> parameterTypes, Type returnType)
        : base(parameterTypes, returnType)
    {
        SpecificationText = specification;
    }

    public string SpecificationText { get; }

    public static StringPatternSpecification Parse(string specification)
    {
        var halves = specification.Split(['>'], 2);

        var parameterNames = halves[0];
        var parameterTypes = ResolveTypeList(parameterNames).ToList();
        
        var returnTypeName = halves[1].Trim();
        var returnType = ResolveType(returnTypeName)!;

        return new(specification, parameterTypes, returnType);
    }

    private static Type ResolveType(string name)
    {
        var genericTypeStart = name.IndexOf('{');
        if (genericTypeStart >= 0)
        {
            var genericTypeParamNames = name[(genericTypeStart + 1)..^1];
            var genericTypeParams = ResolveTypeList(genericTypeParamNames).ToArray();
            
            var genericTypeName = name[..genericTypeStart];
            var genericBaseType = ResolveType(genericTypeName);
            
            var type = genericBaseType.MakeGenericType(genericTypeParams);
            return type;
        }

        return name switch
        {
            "set" => typeof(HashSet<>),
            "list" => typeof(List<>),
            "int" => typeof(int),
            "string" => typeof(string),
            _ => Type.GetType(name)!
        };
    }

    private static IEnumerable<Type> ResolveTypeList(string names)
    {
        return names
            .Split(',')
            .Select(n => n.Trim())
            .Select(ResolveType);
    }

    public override string ToString() => SpecificationText;
}
