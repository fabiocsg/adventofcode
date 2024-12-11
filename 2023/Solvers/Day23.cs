using System.Collections.Immutable;

namespace aoc2023.Solvers;

[Day(23)]
internal sealed class Day23 : IMrWolf
{
    public object SolvePart1(string input)
        => Solve(input);

    // takes about 60s in part2, too slow.. rework?
    public object SolvePart2(string input)
        => Solve(input, true);

    private static int Solve(string input, bool climbSlopes = false)
    {
        var (start, end, links) = ParseMap(input, climbSlopes);

        var roadMap = links
            .GroupBy(l => l.From)
            .ToDictionary(g => g.Key);

        var endLink = links.Single(l => l.To == end);

        var maxLength = 0;
        var queue = new Queue<Context>();
        queue.Enqueue(new Context(start, ImmutableHashSet.Create(start), 0));

        while (queue.TryDequeue(out var current))
        {
            if (current.Pos == end)
            {
                maxLength = Math.Max(maxLength, current.TotalDistance);
                continue;
            }

            foreach (var link in roadMap[current.Pos].Where(l => !current.Visited.Contains(l.To)))
            {
                // the only path to the end is from a single node..
                // if that node is visited and the destination is not the end node we
                // can discard the path because it will never reach the end node.
                // (this condition cut the execution time for part2 from ~60s to ~30s)
                if (link.From == endLink.From && link != endLink)
                {
                    continue;
                }

                queue.Enqueue(new Context
                    (
                        link.To,
                        current.Visited.Add(link.To),
                        current.TotalDistance + link.Distance
                    )
                );
            }
        }

        return maxLength + 1;
    }

    private static (Pos start, Pos end, HashSet<Link> links) ParseMap(string input, bool climbSlopes = false)
    {
        var grid = ParseGrid(input);
        var start = GetStartPosition(grid);
        var end = GetEndPosition(grid);

        var links = new HashSet<Link>();
        var queue = new Queue<Pos>();
        queue.Enqueue(start);

        while (queue.TryDequeue(out var from))
        {
            foreach (var (to, distance) in Navigate(from, end, grid, climbSlopes))
            {
                var link = new Link(from, to, distance);
                links.Add(link);

                if (link.To != end && links.All(p => p.From != link.To))
                {
                    queue.Enqueue(to);
                }
            }
        }

        return (start, end, links);
    }

    private static IEnumerable<(Pos to, int distance)> Navigate(Pos from, Pos end, char[][] grid, bool climbSlopes = false)
    {
        var paths = GetNextPositions(from, grid, climbSlopes);

        foreach (var path in paths)
        {
            var visited = new List<Pos> {from};
            var next = path;

            for (var steps = 1;; steps++)
            {
                var nextPositions = GetNextPositions(next, grid, climbSlopes)
                    .Where(p => !visited.Contains(p))
                    .ToArray();

                if (nextPositions.Length > 1)
                {
                    yield return (next, steps);

                    break;
                }

                if (nextPositions.Length == 0)
                {
                    break;
                }

                visited.Add(next);
                next = nextPositions[0];

                if (next == end)
                {
                    yield return (next, steps);

                    break;
                }
            }
        }
    }

    private static IEnumerable<Pos> GetNextPositions(Pos current, char[][] grid, bool climbSlopes = false)
        => (grid[current.Y][current.X] switch
            {
                '^' when !climbSlopes => new[] {current.Up},
                '>' when !climbSlopes => new[] {current.Right},
                'v' when !climbSlopes => new[] {current.Down},
                '<' when !climbSlopes => new[] {current.Left},
                _ => new[] {current.Up, current.Right, current.Down, current.Left},
            })
            .Where(p => IsValidPosition(p, grid));

    private static char[][] ParseGrid(string source)
        => source.Split('\n')
            .Select(x => x.ToCharArray())
            .ToArray();

    private static Pos GetStartPosition(char[][] grid)
        => new(0, Array.FindIndex(grid[0], x => x == '.'));

    private static Pos GetEndPosition(char[][] grid)
        => new(grid.Length - 1, Array.FindIndex(grid[^1], x => x == '.'));

    private static bool IsValidPosition(Pos pos, char[][] grid)
        => IsInBounds(pos, grid) && grid[pos.Y][pos.X] != '#';

    private static bool IsInBounds(Pos pos, char[][] grid)
        => pos.Y >= 0 && pos.Y < grid.Length
            && pos.X >= 0 && pos.X < grid[0].Length;

    private sealed record Pos(int Y, int X)
    {
        public Pos Up => this with {Y = Y - 1};
        public Pos Right => this with {X = X + 1};
        public Pos Down => this with {Y = Y + 1};
        public Pos Left => this with {X = X - 1};

        public override string ToString() => $"Y = {Y}; X = {X}";
    }

    private sealed record Context(Pos Pos, ImmutableHashSet<Pos> Visited, int TotalDistance);

    private sealed record Link(Pos From, Pos To, int Distance);
}
