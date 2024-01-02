namespace aoc2023.Solvers;

[Day(22)]
internal sealed class Day22 : IMrWolf
{
    public object SolvePart1(string input)
        => Solve(input).Count(c => c.Value == 0);

    public object SolvePart2(string input)
        => Solve(input).Sum(c => c.Value);

    private static Dictionary<Brick, int> Solve(string input)
    {
        var bricks = ParseBricks(input)
            .OrderBy(b => b.Z.Start)
            .ToArray();

        ApplyGravity(bricks);
        var supports = GetSupports(bricks);
        return CalculateCollapsing(bricks, supports);
    }

    private static void ApplyGravity(Brick[] bricks)
    {
        for (var i = 0; i < bricks.Length; i++)
        {
            var falling = bricks[i];

            var support = bricks
                .Take(i)
                .Where(b => OverlapsHorizontally(falling, b))
                .MaxBy(b => b.Z.End);

            var newBase = support != null ? support.Z.End + 1 : 1;
            bricks[i] = falling with {Z = new Range(newBase, newBase + falling.Z.End - falling.Z.Start)};
        }
    }

    private static Dictionary<Brick, Support> GetSupports(Brick[] bricks)
    {
        var result = new Dictionary<Brick, Support>();

        for (var i = 0; i < bricks.Length; i++)
        {
            var current = bricks[i];

            var below = bricks
                .Take(i)
                .Where(b => OverlapsHorizontally(current, b)
                    && b.Z.End == current.Z.Start - 1
                ).ToArray();

            var above = bricks
                .Skip(i + 1)
                .Where(b => OverlapsHorizontally(current, b)
                    && b.Z.Start == current.Z.End + 1
                ).ToArray();

            result.Add(current, new Support(below, above));
        }

        return result;
    }

    private static Dictionary<Brick, int> CalculateCollapsing(Brick[] bricks, Dictionary<Brick, Support> supports)
    {
        var result = new Dictionary<Brick, int>();

        foreach (var brickRemoved in bricks)
        {
            var removed = new HashSet<Brick> {brickRemoved};
            var queue = new Queue<Brick>();
            queue.Enqueue(brickRemoved);

            while (queue.TryDequeue(out var brick))
            {
                var above = supports[brick].Above;

                foreach (var falling in above.Where(a => supports[a].Below.All(b => removed.Contains(b))))
                {
                    removed.Add(falling);
                    queue.Enqueue(falling);
                }
            }

            result.Add(brickRemoved, removed.Count - 1);
        }

        return result;
    }

    private static bool OverlapsHorizontally(Brick b1, Brick b2)
        => Overlaps(b1.X, b2.X) && Overlaps(b1.Y, b2.Y);

    private static bool Overlaps(Range r1, Range r2)
        => r1.Start <= r2.End && r2.Start <= r1.End;

    private static IEnumerable<Brick> ParseBricks(string input)
        => input.Split('\n')
            .Select(line => line
                .Split(new[] {'~', ','}, StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToArray()
            )
            .Select(v => new Brick(new Range(v[0], v[3]), new Range(v[1], v[4]), new Range(v[2], v[5])));

    private sealed record Brick(Range X, Range Y, Range Z);

    private sealed record Range
    {
        public int Start { get; }
        public int End { get; }

        public Range(int start, int end)
            => (Start, End) = (Math.Min(start, end), Math.Max(start, end));
    }

    private sealed record Support(Brick[] Below, Brick[] Above);
}
