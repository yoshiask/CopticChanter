using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CoptLib.Writing;

/// <summary>
/// An interface for basic searches within strings.
/// </summary>
/// <remarks>
/// Functionally, a port of Dart's <see href="https://api.dart.dev/stable/3.5.4/dart-core/Pattern-class.html">Pattern class</see>.
/// </remarks>
public abstract class Pattern
{
    public abstract IEnumerable<PatternMatch> AllMatches(string str, int start = 0);

    public abstract PatternMatch? MatchAsPrefix(string str, int start = 0);

    public static implicit operator Pattern(Regex regex) => new RegexPattern(regex);
    public static implicit operator Pattern(string expression) => new StringPattern(expression);
}

public sealed class RegexPattern(Regex regex) : Pattern
{
    public Regex RegularExpression { get; } = regex;

    public override IEnumerable<PatternMatch> AllMatches(string str, int start = 0)
    {
        return RegularExpression.Matches(str, start)
            .Cast<Match>()
            .Where(m => m.Success)
            .Select(m => CreateMatch(m, str));
    }

    public override PatternMatch? MatchAsPrefix(string str, int start = 0)
    {
        var match = RegularExpression.Match(str, start);

        return match.Success && match.Index == start
            ? CreateMatch(match, str)
            : null;
    }

    public static implicit operator RegexPattern(Regex regex) => new(regex);
    public static implicit operator RegexPattern(string regex) => new(new Regex(regex));

    private PatternMatch CreateMatch(Match reMatch, string str)
    {
        Dictionary<int, string?> groups = new(reMatch.Groups.Count);
        for (int i = 0; i < reMatch.Groups.Count; i++)
        {
            var group = reMatch.Groups[i];
            groups[i] = group.Success ? group.Value : null;
        }

        return new PatternMatch
        {
            Start = reMatch.Index,
            End = reMatch.Index + reMatch.Length,
            Input = str,
            Pattern = this,
            Groups = groups,
        };
    }

    public override string ToString() => RegularExpression.ToString();
}

public sealed class StringPattern(string expression) : Pattern
{
    public string Expression { get; } = expression;

    public override IEnumerable<PatternMatch> AllMatches(string str, int start = 0)
    {
        int searchIndex = start;
        while (searchIndex < str.Length)
        {
            searchIndex = str.IndexOf(Expression, searchIndex);
            if (searchIndex < 0)
                break;

            yield return new PatternMatch
            {
                Start = searchIndex,
                End = searchIndex + Expression.Length,
                Input = str,
                Pattern = this,
                Groups = [],
            };
        }
    }

    public override PatternMatch? MatchAsPrefix(string str, int start = 0)
    {
        var matchIndex = str.IndexOf(Expression, start);

        if (matchIndex != start)
            return null;

        return new PatternMatch
        {
            Start = matchIndex,
            End = matchIndex + Expression.Length,
            Input = str,
            Pattern = this,
            Groups = [],
        };
    }

    public static implicit operator StringPattern(string expression) => new(expression);

    public override string ToString() => Expression;
}

public sealed class PatternMatch
{
    public int Start { get; init; }
    public int End { get; init; }
    public required string Input { get; init; }
    public required Pattern Pattern { get; init; }
    public required Dictionary<int, string?> Groups { get; init; }
}
