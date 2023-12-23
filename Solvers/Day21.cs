using aoc2023.Core;

namespace aoc2023.Solvers;

[Day(21)]
internal sealed class Day21 : IMrWolf
{
    public object SolvePart1(string input)
    {
        var grid = ParseGrid(input);
        var startPos = GetStartPosition(grid);
        var startSet = new HashSet<Pos> {startPos};
        var resultSet = Solve(startSet, grid, 64);
        return resultSet.Count;
    }

    public object SolvePart2(string input)
    {
        var grid = ParseGrid(input);
        var startPos = GetStartPosition(grid);
        var startSet = new HashSet<Pos> {startPos};
        
        // Assuming the grid is a square
        var gridSize = grid.Length;
        var gridsCount = 26501365 / gridSize;
        var offset = 26501365 % gridSize;

        var results = Enumerable.Range(0, 3)
            .Select(n => Solve(startSet, grid, n * gridSize + offset, true).Count)
            .ToArray();

        // ax^2 + bx + c
        // n=0 => a0^2+0b+c = c
        // n=1 => a1^2+1b+c = a+b+c
        // n=2 => a2^2+2b+c = 4a+2b+c
        long n0 = results[0]; // c
        long n1 = results[1]; // a+b+c
        long n2 = results[2]; // 4a+2b+c
        
        // find the coefficients to solve for N = gridsCount
        var c = n0;
        var t1 = n1 - c; // a+b = a+b+c - c
        var t2 = n2 - c; // 4a+2b = 4a+2b+c - c
        var t3 = t2 - 2*t1; // 2a = 4a+2b - 2*(a+b)
        var a = t3 / 2; // a = 2a / 2
        var b = t1 - a; // b = a+b - a

        return a * Math.Pow(gridsCount, 2) + b * gridsCount + c;
    }

    private static HashSet<Pos> Solve(HashSet<Pos> positions, char[][] grid, int maxSteps, bool repeatGrid = false)
    {
        for (var i = 0; i < maxSteps; i++)
        {
            positions = positions
                .SelectMany(NextPositions)
                .Where(pos => IsValidPos(pos, grid, repeatGrid))
                .ToHashSet();
        }

        return positions;
    }

    private static IEnumerable<Pos> NextPositions(Pos current)
    {
        yield return current with {Y = current.Y - 1};
        yield return current with {Y = current.Y + 1};
        yield return current with {X = current.X - 1};
        yield return current with {X = current.X + 1};
    }

    private static char[][] ParseGrid(string source)
        => source.Split('\n')
            .Select(x => x.ToCharArray())
            .ToArray();

    private static Pos GetStartPosition(char[][] grid)
    {
        var startRow = Array.FindIndex(grid, r => r.Contains('S'));
        var startColumn = Array.FindIndex(grid[startRow], x => x == 'S');
        return new Pos(startRow, startColumn);
    }

    private static bool IsValidPos(Pos pos, char[][] grid, bool repeatGrid)
    {
        if (!repeatGrid)
        {
            return IsInBounds(pos, grid) && grid[pos.Y][pos.X] != '#';
        }

        var gridHeight = grid.Length;
        var gridWidth = grid[0].Length;
        var y = pos.Y % gridHeight + (pos.Y % gridHeight >= 0 ? 0 : gridHeight);
        var x = pos.X % gridWidth + (pos.X % gridWidth >= 0 ? 0 : gridWidth);
        return grid[y][x] != '#';
    }

    private static bool IsInBounds(Pos pos, char[][] grid)
        => pos.Y >= 0 && pos.Y < grid.Length
            && pos.X >= 0 && pos.X < grid[0].Length;

    private sealed record Pos(int Y, int X);
}
