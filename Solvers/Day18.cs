using System.Globalization;
using aoc2023.Core;

namespace aoc2023.Solvers;

[Day(18)]
internal sealed class Day18 : IMrWolf
{
    public object SolvePart1(string input)
        => Solve(input, ParseInstruction);

    public object SolvePart2(string input)
        => Solve(input, ParseInstructionFromHex);

    private static long Solve(string input, Func<string, Instruction> rowParser)
    {
        var instructions = ParseInput(input, rowParser);
        var points = new List<Pos>();
        var pos = new Pos(0, 0);

        foreach (var instruction in instructions)
        {
            for (var i = 0; i < instruction.Length; i++)
            {
                pos = NextPosition(pos, instruction.Dir);
                points.Add(pos);
            }
        }

        // https://en.wikipedia.org/wiki/Shoelace_formula
        // https://en.wikipedia.org/wiki/Pick%27s_theorem
        var area = CalculateArea(points);
        return area + points.Count / 2 + 1;
    }

    private static Pos NextPosition(Pos pos, string dir)
        => dir switch
        {
            "U" => pos with {Y = pos.Y - 1},
            "D" => pos with {Y = pos.Y + 1},
            "L" => pos with {X = pos.X - 1},
            "R" => pos with {X = pos.X + 1},
            _ => throw new ShouldNeverHappenException(),
        };

    private static long CalculateArea(List<Pos> source)
    {
        long sum = 0;

        for (var i = 0; i < source.Count - 1; i++)
        {
            sum += source[i].X * source[i + 1].Y - source[i + 1].X * source[i].Y;
        }

        return Math.Abs(sum + source[^1].X * source[0].Y - source[0].X * source[^1].Y) / 2;
    }

    private static List<Instruction> ParseInput(string input, Func<string, Instruction> rowParser)
        => input.Split('\n').Select(rowParser).ToList();

    private static Instruction ParseInstruction(string source)
    {
        var parts = source.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return new Instruction(parts[0], int.Parse(parts[1]));
    }

    private static Instruction ParseInstructionFromHex(string source)
    {
        var parts = source.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var length = int.Parse(parts[2][2..^2], NumberStyles.HexNumber);

        var dir = parts[2][^2] switch
        {
            '0' => "R",
            '1' => "D",
            '2' => "L",
            '3' => "U",
            _ => throw new ShouldNeverHappenException(),
        };

        return new Instruction(dir, length);
    }

    private sealed record Instruction(string Dir, int Length);

    private sealed record Pos(long Y, long X);
}
