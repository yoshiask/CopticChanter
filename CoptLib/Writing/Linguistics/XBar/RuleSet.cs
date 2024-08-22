using CoptLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CoptLib.Writing.Linguistics.XBar;

public class RuleSet(List<Tag>? rootTags = null, List<Rule>? rules = null)
{
    public List<Tag> RootTags { get; } = rootTags ?? [];

    public List<Rule> Rules { get; } = rules ?? [];
}

public class Rule(Tag parent, List<string> childrenRules)
{
    public Tag Parent { get; } = parent;

    public List<ChildrenRuleExpression> ChildrenRuleExpressions { get; } = childrenRules.Select(ChildrenRuleExpression.Parse).ToList();

    public override string ToString()
    {
        StringBuilder sb = new(Parent.ToString());
        sb.Append(" → ");

        var body = string.Join(" + ", ChildrenRuleExpressions);
        sb.Append(body);

        return sb.ToString();
    }

    public bool Validate(List<Tag> children)
    {
        return true;
    }
}

public class ChildrenRuleExpression : Expression
{
    public Expression Expression { get; }

    public override ExpressionType NodeType => ExpressionType.Extension;

    private ChildrenRuleExpression(Expression expression)
    {
        Expression = expression;
    }

    public static ChildrenRuleExpression Parse(string str) => new(InnerParse(str));

    private static Expression InnerParse(string str)
    {
        // Optional
        if (str[0] == '(' && str[^1] == ')')
        {
            var inner = InnerParse(str[1..^1]);
            return new OptionalExpression(inner);
        }

        // Multiple options
        if (str[0] == '{' && str[^1] == '}')
        {
            var inner = str[1..^2].Split('/').Select(InnerParse);
            return NewArrayInit(typeof(Tag), inner);
        }

        // Lone tag
        var tag = Tag.Parse(str);
        return Constant(tag);
    }

    public override string ToString() => ToString(Expression);

    private static string ToString(Expression expr)
    {
        if (expr is OptionalExpression optionalExpr)
            return $"({ToString(optionalExpr.Inner)})";
        if (expr is NewArrayExpression arrayExpr)
            return $"{{{string.Join("/", arrayExpr.Expressions.Select(ToString))}}}";

        return expr.ToString();
    }
}

public class OptionalExpression(Expression inner) : Expression
{
    public Expression Inner { get; } = inner;

    public override string ToString() => $"{base.ToString()}?";
}
