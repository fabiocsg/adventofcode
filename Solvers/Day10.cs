using aoc2023.Core;

namespace aoc2023.Solvers;

[Day(10)]
internal sealed class Day10 : IMrWolf
{
    public object SolvePart1(string input)
        => new Grid(input).TotalSteps / 2;

    public object SolvePart2(string input)
        => new Grid(input).ToString()
            .Split('\n')
            .Select(Transform)
            .Select(x => x.Split('|').Where((_, i) => i % 2 != 0))
            .Select(x => string.Join(string.Empty, x).Length)
            .Sum();

    private static string Transform(string source)
        => source
            .Replace("-", string.Empty)
            .Replace("F7", string.Empty)
            .Replace("LJ", string.Empty)
            .Replace("FJ", "|")
            .Replace("L7", "|");

    private sealed class Grid
    {
        private readonly char[][] _grid;
        private readonly HashSet<Pos> _visited = new();

        public Grid(string source)
        {
            _grid = source.Split('\n')
                .Select(x => x.ToCharArray())
                .ToArray();

            Navigate();
            Sanitize();
        }

        public int TotalSteps => _visited.Count;
        public override string ToString() => string.Join('\n', _grid.Select(x => new string(x)));

        private void Navigate()
        {
            var start = GetStartPosition();
            _visited.Add(start);
            var current = start;
            var from = Dir.North;

            do
            {
                (current, from) = Next(current, from);
                _visited.Add(current);
            } while (current != start);
        }

        private Pos GetStartPosition()
        {
            var startRow = Array.FindIndex(_grid, r => r.Contains('S'));
            var startColumn = Array.FindIndex(_grid[startRow], x => x == 'S');
            return new Pos(startRow, startColumn);
        }

        private (Pos pos, Dir from) Next(Pos pos, Dir from)
            => _grid[pos.Row][pos.Col] switch
            {
                '-' when from == Dir.West => (pos with {Col = pos.Col + 1}, Dir.West),
                '-' when from == Dir.East => (pos with {Col = pos.Col - 1}, Dir.East),
                '|' when from == Dir.North => (pos with {Row = pos.Row + 1}, Dir.North),
                '|' when from == Dir.South => (pos with {Row = pos.Row - 1}, Dir.South),
                'J' when from == Dir.West => (pos with {Row = pos.Row - 1}, Dir.South),
                'J' when from == Dir.North => (pos with {Col = pos.Col - 1}, Dir.East),
                'F' when from == Dir.South => (pos with {Col = pos.Col + 1}, Dir.West),
                'F' when from == Dir.East => (pos with {Row = pos.Row + 1}, Dir.North),
                '7' when from == Dir.West => (pos with {Row = pos.Row + 1}, Dir.North),
                '7' when from == Dir.South => (pos with {Col = pos.Col - 1}, Dir.East),
                'L' when from == Dir.North => (pos with {Col = pos.Col + 1}, Dir.West),
                'L' when from == Dir.East => (pos with {Row = pos.Row - 1}, Dir.South),
                'S' => FirstMove(pos),
                _ => throw new ShouldNeverHappenException(),
            };

        private (Pos pos, Dir from) FirstMove(Pos pos)
        {
            var possibleMoves = new[]
                {
                    (pos with {Row = pos.Row - 1}, "|7F", Dir.South),
                    (pos with {Row = pos.Row + 1}, "|JL", Dir.North),
                    (pos with {Col = pos.Col + 1}, "-7J", Dir.West),
                    (pos with {Col = pos.Col - 1}, "-LF", Dir.East),
                }
                .Where(x => CanMove(x.Item1, x.Item2))
                .ToList(); // should always have 2 elements

            _grid[pos.Row][pos.Col] = possibleMoves.Select(x => x.Item3).Aggregate((a, b) => a | b) switch
            {
                Dir.NorthEast => '7',
                Dir.NorthWest => 'F',
                Dir.SouthEast => 'L',
                Dir.SouthWest => 'J',
                Dir.NorthSouth => '|',
                Dir.EastWest => '-',
                _ => throw new ShouldNeverHappenException(),
            };

            return possibleMoves.Select(x => (x.Item1, x.Item3)).First();
        }

        private bool CanMove(Pos target, string search)
            => target.Col >= 0 && target.Col < _grid[0].Length
                && target.Row >= 0 && target.Row < _grid.Length
                && search.Contains(_grid[target.Row][target.Col]);

        private void Sanitize()
        {
            for (var i = 0; i < _grid.Length; i++)
            {
                for (var j = 0; j < _grid[i].Length; j++)
                {
                    if (!_visited.Contains(new Pos(i, j)))
                    {
                        _grid[i][j] = '.';
                    }
                }
            }
        }
    }

    private sealed record Pos(int Row, int Col);

    [Flags]
    private enum Dir
    {
        North = 1,
        East = 2,
        West = 4,
        South = 8,
        NorthEast = North | East,
        NorthWest = North | West,
        SouthEast = South | East,
        SouthWest = South | West,
        NorthSouth = North | South,
        EastWest = East | West,
    }
}
