using System.Globalization;

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
            pos = NextPosition(pos, instruction);
            points.Add(pos);
        }

        // https://en.wikipedia.org/wiki/Shoelace_formula
        // https://en.wikipedia.org/wiki/Pick%27s_theorem
        var area = CalculateArea(points);
        var perimeter = instructions.Sum(i => i.Length);
        return area + perimeter / 2 + 1;
    }

    private static Pos NextPosition(Pos pos, Instruction instruction)
        => instruction.Dir switch
        {
            "U" => pos with {Y = pos.Y - instruction.Length},
            "D" => pos with {Y = pos.Y + instruction.Length},
            "L" => pos with {X = pos.X - instruction.Length},
            "R" => pos with {X = pos.X + instruction.Length},
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
