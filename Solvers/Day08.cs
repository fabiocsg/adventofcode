using System.Text.RegularExpressions;

namespace aoc2023.Solvers;

[Day(8)]
internal sealed class Day08 : IMrWolf
{
    private const string ParseRegex = "[A-Z0-9]{3}";

    public object SolvePart1(string input)
        => SolveWithNavigator(input, NavigateOne);

    public object SolvePart2(string input)
        => SolveWithNavigator(input, NavigateMany);

    private static long SolveWithNavigator(string input, Func<string, Map, long> navigate)
    {
        var parts = input.Split('\n');
        var sequence = parts[0];
        var map = ParseMap(parts[2..]);
        return navigate(sequence, map);
    }

    private static long NavigateOne(string sequence, Map map)
        => CountSteps(sequence, map, "AAA", x => x == "ZZZ");

    // Lazy solution. In the given input the sequence always starts back after completing.
    private static long NavigateMany(string sequence, Map map)
        => map.Keys
            .Where(key => key.EndsWith("A"))
            .Select(pos => CountSteps(sequence, map, pos, x => x.EndsWith("Z")))
            .Aggregate(Lcm);

    private static long CountSteps(string seq, Map map, string position, Func<string, bool> endPredicate)
    {
        var steps = 0;

        while (!endPredicate(position))
        {
            position = map.Next(position, seq[steps % seq.Length]);
            steps++;
        }

        return steps;
    }

    private static Map ParseMap(IEnumerable<string> source)
    {
        var nodes = new Map();

        foreach (var str in source)
        {
            var matches = Regex.Matches(str, ParseRegex)
                .Select(m => m.Value)
                .ToArray();

            nodes.Add(matches[0], (matches[1], matches[2]));
        }

        return nodes;
    }

    // https://en.wikipedia.org/wiki/Least_common_multiple#Calculation
    private static long Lcm(long n1, long n2)
        => n1 * n2 / Gcd(n1, n2);

    // https://en.wikipedia.org/wiki/Euclidean_algorithm#Implementations
    private static long Gcd(long n1, long n2)
        => n2 == 0
            ? n1
            : Gcd(n2, n1 % n2);

    private sealed class Map : Dictionary<string, (string Left, string Right)>
    {
        public string Next(string from, char direction) => direction switch
        {
            'L' => this[from].Left,
            'R' => this[from].Right,
            _ => throw new NotSupportedException(),
        };
    }
}
