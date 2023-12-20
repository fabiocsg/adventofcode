using System.Text.RegularExpressions;
using aoc2023.Core;

namespace aoc2023.Solvers;

[Day(19)]
internal sealed class Day19 : IMrWolf
{
    public object SolvePart1(string input)
    {
        var (patterns, parts) = Parse(input);

        return parts
            .Where(part => patterns.Any(pattern => pattern.Validate(part)))
            .Sum(p => p.X + p.M + p.A + p.S);
    }

    public object SolvePart2(string input)
        => Parse(input).patterns.Sum(p => p.X.Count * p.M.Count * p.A.Count * p.S.Count);

    private static (List<Pattern> patterns, List<Part> parts) Parse(string input)
    {
        var (workflows, parts) = ParseInput(input);
        var valid = new List<Pattern>();
        var range = new Range(1, 4000);
        var queue = new Queue<Context>();
        queue.Enqueue(new Context("in", new Pattern(range, range, range, range)));

        while (queue.Count > 0)
        {
            var ctx = queue.Dequeue();

            foreach (var nextCtx in Process(ctx, workflows[ctx.Name]))
            {
                if (nextCtx.Name == "A")
                {
                    valid.Add(nextCtx.Pattern);
                }
                else if (nextCtx.Name != "R")
                {
                    queue.Enqueue(nextCtx);
                }
            }
        }

        return (valid, parts);
    }

    private static IEnumerable<Context> Process(Context ctx, List<Rule> rules)
    {
        var pattern = ctx.Pattern;

        foreach (var rule in rules)
        {
            if (rule.Condition == null)
            {
                yield return new Context(rule.Destination, pattern);

                continue;
            }

            var (match, remaining) = Split(rule.Condition!, pattern);

            if (match != null)
            {
                yield return new Context(rule.Destination, match);
            }

            if (remaining == null)
            {
                yield break;
            }

            pattern = remaining;
        }
    }

    private static (Pattern? match, Pattern? remaining) Split(Condition condition, Pattern pattern)
    {
        var current = condition.Category switch
        {
            'x' => pattern.X,
            'm' => pattern.M,
            'a' => pattern.A,
            's' => pattern.S,
            _ => throw new ShouldNeverHappenException(),
        };

        if ((condition.Op == '<' && current.Min > condition.Value) || (condition.Op == '>' && current.Max < condition.Value))
        {
            return (null, pattern);
        }

        var (categoryMatch, categoryRamaining) = condition.Op == '<'
            ? (current with {Max = condition.Value - 1}, current with {Min = condition.Value})
            : (current with {Min = condition.Value + 1}, current with {Max = condition.Value});

        return condition.Category switch
        {
            'x' => (pattern with {X = categoryMatch}, pattern with {X = categoryRamaining}),
            'm' => (pattern with {M = categoryMatch}, pattern with {M = categoryRamaining}),
            'a' => (pattern with {A = categoryMatch}, pattern with {A = categoryRamaining}),
            's' => (pattern with {S = categoryMatch}, pattern with {S = categoryRamaining}),
            _ => throw new ShouldNeverHappenException(),
        };
    }

    private static (Dictionary<string, List<Rule>> workflows, List<Part> parts) ParseInput(string source)
    {
        var split = source.Split("\n\n");
        var workflows = split[0].Split('\n').Select(ParseWorkflow).ToDictionary(x => x.name, x => x.rules);
        var parts = split[1].Split('\n').Select(ParsePart).ToList();
        return (workflows, parts);
    }

    private static (string name, List<Rule> rules) ParseWorkflow(string source)
    {
        var split = source.Split('{', '}');
        return (split[0], split[1].Split(',').Select(ParseRule).ToList());
    }

    private static Rule ParseRule(string source)
    {
        var i = source.IndexOf(':');

        if (i == -1)
        {
            return new Rule(null, source);
        }

        var condition = new Condition(source[0], source[1], int.Parse(source[2..i]));
        return new Rule(condition, source[(i + 1)..]);
    }

    private static Part ParsePart(string source)
    {
        var n = Regex.Matches(source, @"\d+").Select(m => int.Parse(m.Value)).ToArray();
        return new Part(n[0], n[1], n[2], n[3]);
    }

    private sealed record Rule(Condition? Condition, string Destination);

    private sealed record Condition(char Category, char Op, int Value);

    private sealed record Part(int X, int M, int A, int S);

    private sealed record Range(int Min, int Max)
    {
        public long Count => Max - Min + 1;
        public bool Contains(int n) => n >= Min && n <= Max;
    }

    private sealed record Pattern(Range X, Range M, Range A, Range S)
    {
        public bool Validate(Part p) => X.Contains(p.X) && M.Contains(p.M) && A.Contains(p.A) && S.Contains(p.S);
    }

    private sealed record Context(string Name, Pattern Pattern);
}
