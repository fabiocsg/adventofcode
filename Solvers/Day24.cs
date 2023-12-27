using aoc2023.Core;

namespace aoc2023.Solvers;

[Day(24)]
internal sealed class Day24 : IMrWolf
{
    private const long BoundsMin = 200_000_000_000_000;
    private const long BoundsMax = 400_000_000_000_000;
    
    public object SolvePart1(string input)
    {
        var hailstones = ParseHailstones(input);

        return GetPairs(hailstones)
            .Count(pair => IntersectsWithinBounds(pair.Item1, pair.Item2, BoundsMin, BoundsMax));
    }

    public object SolvePart2(string input)
        => "sorry mom, I give up..";

    private static bool IntersectsWithinBounds(Hailstone h1, Hailstone h2, long min, long max)
    {
        var determinant = h1.Vel.X * h2.Vel.Y - h1.Vel.Y * h2.Vel.X;

        if (determinant == 0)
        {
            return false;
        }

        var dy = h1.Pos.Y - h2.Pos.Y;
        var dx = h2.Pos.X - h1.Pos.X;
        var t1 = (h2.Vel.X * dy + h2.Vel.Y * dx) / determinant;
        var t2 = (h1.Vel.X * dy + h1.Vel.Y * dx) / determinant;

        if (t1 < 0 || t2 < 0)
        {
            return false; 
        }

        var x = h2.Pos.X + h2.Vel.X * t2;
        var y = h2.Pos.Y + h2.Vel.Y * t2;
        
        return x >= min && x <= max && y >= min && y <= max;
    }

    private static (T, T)[] GetPairs<T>(T[] source)
        => source
            .SelectMany((a, i) => source.Skip(i + 1).Select(b => (a, b)))
            .ToArray();

    private static Hailstone[] ParseHailstones(string input)
        => input.Split('\n')
            .Select(ParseHailstone)
            .ToArray();

    private static Hailstone ParseHailstone(string source)
    {
        var v = source.Split(new[] {'@', ','}, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToArray();

        return new Hailstone(new Vec(v[0], v[1], v[2]), new Vec(v[3], v[4], v[5]));
    }

    private sealed record Hailstone(Vec Pos, Vec Vel);

    private sealed record Vec(long X, long Y, long Z);
}
