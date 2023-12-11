using aoc2023.Core;

namespace aoc2023.Solvers;

[Day(11)]
internal sealed class Day11 : IMrWolf
{
    public object SolvePart1(string input)
        => Solve(input, 2);

    public object SolvePart2(string input)
        => Solve(input, 1000000);

    private static long Solve(string input, int emptyDistance)
    {
        var galaxies = ParseGalaxies(input);

        var emptyRows = GetEmpty(galaxies, pos => pos.Row);
        var emptyCols = GetEmpty(galaxies, pos => pos.Col);

        return GetPairs(galaxies)
            .Select(pair => GetDistance(pair, emptyRows, emptyCols, emptyDistance))
            .Sum();
    }

    private static List<Pos> ParseGalaxies(string source)
        => source.Split('\n')
            .Select(x => x.ToCharArray())
            .SelectMany((x, i) => x.Select((c, j) => new {Char = c, Pos = new Pos(i, j)}))
            .Where(x => x.Char == '#')
            .Select(x => x.Pos)
            .ToList();

    private static List<int> GetEmpty(List<Pos> source, Func<Pos, int> predicate)
        => Enumerable.Range(0, source.Max(predicate))
            .Where(n => source.All(pos => predicate(pos) != n))
            .ToList();

    private static List<(T, T)> GetPairs<T>(List<T> source)
        => source
            .SelectMany((a, i) => source.Skip(i + 1).Select(b => (a, b)))
            .ToList();

    private static long GetDistance((Pos, Pos) pair, List<int> emptyRows, List<int> emptyCols, int emptyDistance)
    {
        var xDiff = GetDistance(pair.Item1.Col, pair.Item2.Col, emptyCols, emptyDistance);
        var yDiff = GetDistance(pair.Item1.Row, pair.Item2.Row, emptyRows, emptyDistance);
        return xDiff + yDiff;
    }

    private static long GetDistance(int a, int b, List<int> empty, int emptyDistance)
        => Enumerable.Range(Math.Min(a, b), Math.Abs(a - b))
            .Select(n => (long) (empty.Contains(n) ? emptyDistance : 1))
            .Sum();

    private sealed record Pos(int Row, int Col);
}
