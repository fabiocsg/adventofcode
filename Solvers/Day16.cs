using aoc2023.Core;

namespace aoc2023.Solvers;

[Day(16)]
internal sealed class Day16 : IMrWolf
{
    public object SolvePart1(string input)
        => SolveFrom(new Visit(0, 0, Dir.Left), ParseGrid(input));

    public object SolvePart2(string input)
    {
        var grid = ParseGrid(input);
        var y0 = Enumerable.Range(0, grid.Length).Select(y => new Visit(y, 0, Dir.Left));
        var yn = Enumerable.Range(0, grid.Length).Select(y => new Visit(y, grid[0].Length - 1, Dir.Right));
        var x0 = Enumerable.Range(0, grid[0].Length).Select(x => new Visit(0, x, Dir.Up));
        var xn = Enumerable.Range(0, grid[0].Length).Select(x => new Visit(grid.Length - 1, x, Dir.Down));

        return y0.Concat(yn).Concat(x0).Concat(xn)
            .Select(v => SolveFrom(v, grid))
            .Max();
    }

    private static int SolveFrom(Visit visit, char[][] grid)
    {
        var visited = new HashSet<Visit>();
        ProcessTile(visit, visited, grid);

        return visited
            .DistinctBy(tile => (tile.X, tile.Y))
            .Count();
    }

    private static void ProcessTile(Visit visit, HashSet<Visit> visited, char[][] grid)
    {
        if (!IsInBounds(visit, grid) || !visited.Add(visit))
        {
            return;
        }

        Action next = grid[visit.Y][visit.X] switch
        {
            '.' when visit.From == Dir.Up => () => ProcessTile(visit with {Y = visit.Y + 1}, visited, grid),
            '.' when visit.From == Dir.Down => () => ProcessTile(visit with {Y = visit.Y - 1}, visited, grid),
            '.' when visit.From == Dir.Left => () => ProcessTile(visit with {X = visit.X + 1}, visited, grid),
            '.' when visit.From == Dir.Right => () => ProcessTile(visit with {X = visit.X - 1}, visited, grid),
            '\\' when visit.From == Dir.Up => () => ProcessTile(new Visit(visit.Y, visit.X + 1, Dir.Left), visited, grid),
            '\\' when visit.From == Dir.Down => () => ProcessTile(new Visit(visit.Y, visit.X - 1, Dir.Right), visited, grid),
            '\\' when visit.From == Dir.Right => () => ProcessTile(new Visit(visit.Y - 1, visit.X, Dir.Down), visited, grid),
            '\\' when visit.From == Dir.Left => () => ProcessTile(new Visit(visit.Y + 1, visit.X, Dir.Up), visited, grid),
            '/' when visit.From == Dir.Up => () => ProcessTile(new Visit(visit.Y, visit.X - 1, Dir.Right), visited, grid),
            '/' when visit.From == Dir.Down => () => ProcessTile(new Visit(visit.Y, visit.X + 1, Dir.Left), visited, grid),
            '/' when visit.From == Dir.Right => () => ProcessTile(new Visit(visit.Y + 1, visit.X, Dir.Up), visited, grid),
            '/' when visit.From == Dir.Left => () => ProcessTile(new Visit(visit.Y - 1, visit.X, Dir.Down), visited, grid),
            '-' when visit.From is Dir.Left or Dir.Right => () => ProcessTile(visit with {X = visit.From == Dir.Left ? visit.X + 1 : visit.X - 1}, visited, grid),
            '|' when visit.From is Dir.Down or Dir.Up => () => ProcessTile(visit with {Y = visit.From == Dir.Up ? visit.Y + 1 : visit.Y - 1}, visited, grid),
            '-' when visit.From is Dir.Up or Dir.Down => () =>
            {
                ProcessTile(new Visit(visit.Y, visit.X + 1, Dir.Left), visited, grid);
                ProcessTile(new Visit(visit.Y, visit.X - 1, Dir.Right), visited, grid);
            },
            '|' when visit.From is Dir.Left or Dir.Right => () =>
            {
                ProcessTile(new Visit(visit.Y + 1, visit.X, Dir.Up), visited, grid);
                ProcessTile(new Visit(visit.Y - 1, visit.X, Dir.Down), visited, grid);
            },
            _ => throw new ShouldNeverHappenException(),
        };

        next();
    }

    private static char[][] ParseGrid(string source)
        => source.Split('\n')
            .Select(x => x.ToCharArray())
            .ToArray();

    private static bool IsInBounds(Visit visit, char[][] grid)
        => visit.Y >= 0 && visit.Y < grid.Length
            && visit.X >= 0 && visit.X < grid[0].Length;

    private sealed record Visit(int Y, int X, Dir From);

    private enum Dir
    {
        Up,
        Right,
        Down,
        Left,
    }
}
