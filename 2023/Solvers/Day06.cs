namespace aoc2023.Solvers;

[Day(6)]
internal sealed class Day06 : IMrWolf
{
    public object SolvePart1(string input)
        => Solve(input);

    public object SolvePart2(string input)
        => Solve(input.Replace(" ", string.Empty));

    public int Solve(string input)
        => ParseInput(input)
            .Select(GetWinningTimesCount)
            .Aggregate(1, (a, b) => a * b);

    private static IEnumerable<Race> ParseInput(string input)
    {
        var parts = input.Split('\n');
        var times = ParseRow(parts[0]);
        var distances = ParseRow(parts[1]);

        return times.Zip(distances, (time, distance) => new Race(time, distance));
    }

    private static IEnumerable<long> ParseRow(string row)
        => row.Split(':', ' ')
            .Skip(1)
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(long.Parse);

    private static int GetWinningTimesCount(Race race)
    {
        // time - x = timeRunning
        // timeRunning * x = distance

        // (time - x) * x = distance
        // time*x - x^2 = distance
        // -x^2 + time*x - distance

        var (min, max) = SolveQuadraticEquation(-1, race.Time, -(race.Distance + 1)); // we must win, not draw
        return (int) Math.Floor(max) - (int) Math.Ceiling(min) + 1;
    }

    private static (double min, double max) SolveQuadraticEquation(long a, long b, long c)
    {
        // -b ± √(b^2 - 4ac)
        // -----------------
        //        2a

        var d = Math.Pow(b, 2) - 4 * a * c;
        var x1 = (b - Math.Sqrt(d)) / 2;
        var x2 = (b + Math.Sqrt(d)) / 2;

        var min = Math.Min(x1, x2);
        var max = Math.Max(x1, x2);
        return (min, max);
    }
}

internal sealed record Race(long Time, long Distance);
