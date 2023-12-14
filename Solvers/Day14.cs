using aoc2023.Core;

namespace aoc2023.Solvers;

[Day(14)]
internal sealed class Day14 : IMrWolf
{
    public object SolvePart1(string input)
    {
        var grid = Parse(input);
        Tilt(grid);
        return Sum(grid);
    }

    public object SolvePart2(string input)
    {
        var grid = Parse(input);
        var store = new Dictionary<string, int>();

        for (var i = 1000000000; i > 0; i--)
        {
            for (var j = 0; j < 4; j++)
            {
                Tilt(grid);
                grid = Rotate(grid);
            }

            var key = Stringify(grid);

            if (store.TryAdd(key, i))
            {
                continue;
            }

            // found a loop. skip it as many times as possible.
            i = store[key] % (store[key] - i);
        }

        return Sum(grid);
    }

    private static char[][] Parse(string input)
        => input.Split('\n')
            .Select(x => x.ToCharArray())
            .ToArray();

    private static void Tilt(char[][] grid)
    {
        bool moved;

        do
        {
            moved = false;

            for (var i = 1; i < grid.Length; i++)
            {
                for (var j = 0; j < grid[i].Length; j++)
                {
                    if (grid[i][j] == 'O' && grid[i - 1][j] == '.')
                    {
                        grid[i][j] = '.';
                        grid[i - 1][j] = 'O';
                        moved = true;
                    }
                }
            }
        } while (moved);
    }

    // rotate clockwise ↻ 
    // copy-paste from day13 ❤️
    private static char[][] Rotate(char[][] entries)
        => entries
            .SelectMany(entry => entry.Select((ch, i) => new {Char = ch, Index = i}))
            .GroupBy(x => x.Index)
            .Select(group => group.Select(x => x.Char).Reverse().ToArray())
            .ToArray();

    private static int Sum(char[][] grid)
        => grid.SelectMany((row, i) => row.Select(cell => cell == 'O' ? grid.Length - i : 0)).Sum();

    private static string Stringify(char[][] grid)
        => new(grid.SelectMany(x => x).ToArray());
}
