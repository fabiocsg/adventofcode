namespace aoc2023.Solvers;

[Day(17)]
internal sealed class Day17 : IMrWolf
{
    private Context _ctx = null!;
    private char[][] _grid = null!;
    private PriorityQueue<State, int> _queue = null!;

    public object SolvePart1(string input)
        => Solve(input, new Context(0, 3));

    public object SolvePart2(string input)
        => Solve(input, new Context(4, 10));

    private int Solve(string input, Context ctx)
    {
        _grid = ParseGrid(input);
        _ctx = ctx;
        _queue = new PriorityQueue<State, int>();

        var visited = new HashSet<State>();
        EnqueuePossibleMoves(new State(new Pos(0, 0), Dir.Left, 0), 0);

        while (_queue.TryDequeue(out var state, out var heatLoss))
        {
            if (!visited.Add(state))
            {
                continue;
            }

            if (Process(state, heatLoss, out var total))
            {
                return total;
            }
        }

        throw new ShouldNeverHappenException();
    }

    private bool Process(State state, int heatLoss, out int totalHeatLoss)
    {
        var blockHeatLoss = int.Parse(_grid[state.Position.Y][state.Position.X].ToString());
        totalHeatLoss = blockHeatLoss + heatLoss;

        if (IsDestination(state.Position, _grid))
        {
            return true;
        }

        EnqueuePossibleMoves(state, totalHeatLoss);
        return false;
    }

    private void EnqueuePossibleMoves(State state, int heatLoss)
    {
        var pos = state.Position;
        var up = (Dir.Down, pos with {Y = pos.Y - 1});
        var down = (Dir.Up, pos with {Y = pos.Y + 1});
        var right = (Dir.Left, pos with {X = pos.X + 1});
        var left = (Dir.Right, pos with {X = pos.X - 1});

        var possibleMoves = state.From switch
        {
            Dir.Up => new[] {down, right, left},
            Dir.Right => new[] {down, left, up},
            Dir.Left => new[] {down, right, up},
            Dir.Down => new[] {right, left, up},
        };

        if (state.StraightMoves < _ctx.MinStraight)
        {
            var move = possibleMoves.Single(m => m.Item1 == state.From);

            if (IsInBounds(move.Item2, _grid))
            {
                var nextState = new State(move.Item2, move.Item1, state.StraightMoves + 1);
                _queue.Enqueue(nextState, heatLoss);
            }

            return;
        }

        foreach (var move in possibleMoves)
        {
            if ((state.From == move.Item1 && state.StraightMoves == _ctx.MaxStraight) || !IsInBounds(move.Item2, _grid))
            {
                continue;
            }

            var nextState = new State(move.Item2, move.Item1, state.From == move.Item1 ? state.StraightMoves + 1 : 1);
            _queue.Enqueue(nextState, heatLoss);
        }
    }

    private static char[][] ParseGrid(string source)
        => source.Split('\n')
            .Select(x => x.ToCharArray())
            .ToArray();

    private static bool IsInBounds(Pos pos, char[][] grid)
        => pos.Y >= 0 && pos.Y < grid.Length
            && pos.X >= 0 && pos.X < grid[0].Length;

    private static bool IsDestination(Pos pos, char[][] grid)
        => pos.Y == grid.Length - 1 && pos.X == grid[0].Length - 1;

    private sealed record Pos(int Y, int X);

    private sealed record State(Pos Position, Dir From, int StraightMoves);

    private sealed record Context(int MinStraight, int MaxStraight);

    private enum Dir
    {
        Up,
        Right,
        Down,
        Left,
    }
}
