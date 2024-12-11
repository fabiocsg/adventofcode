namespace aoc2023.Solvers;

[Day(3)]
internal sealed class Day03 : IMrWolf
{
    public object SolvePart1(string input)
    {
        var (parts, symbols) = ParseEngine(input);
        var result = 0;

        foreach (var part in parts)
        {
            var nearSymbols = symbols.Where(s => s.Row >= part.Row - 1 && s.Row <= part.Row + 1);
            var isValid = nearSymbols.Any(s => s.Column >= part.Column - 1 && s.Column <= part.Column + part.Value.Length);

            if (!isValid)
            {
                continue;
            }

            result += int.Parse(part.Value);
        }

        return result;
    }

    public object SolvePart2(string input)
    {
        var (parts, symbols) = ParseEngine(input);
        var result = 0;

        foreach (var symbol in symbols.Where(s => s.Value == '*'))
        {
            var nearParts = parts.Where(e => e.Row >= symbol.Row - 1 && e.Row <= symbol.Row + 1);
            var touchingParts = nearParts.Where(e => symbol.Column >= e.Column - 1 && symbol.Column <= e.Column + e.Value.Length).ToList();

            if (touchingParts.Count != 2)
            {
                continue;
            }

            result += touchingParts.Aggregate(1, (a, b) => a * int.Parse(b.Value));
        }

        return result;
    }

    private static (List<Part> parts, List<Symbol> symbols) ParseEngine(string input)
    {
        var parts = new List<Part>();
        var symbols = new List<Symbol>();
        var lines = input.Split('\n');

        for (var rowIndex = 0; rowIndex < lines.Length; rowIndex++)
        {
            var line = lines[rowIndex];
            var str = string.Empty;

            for (var columnIndex = 0; columnIndex < line.Length; columnIndex++)
            {
                var c = line[columnIndex];

                if (char.IsNumber(c))
                {
                    str += c;
                    continue;
                }

                if (!string.IsNullOrEmpty(str))
                {
                    parts.Add(new Part(str, rowIndex, columnIndex - str.Length));
                }

                str = string.Empty;

                if (c != '.')
                {
                    symbols.Add(new Symbol(c, rowIndex, columnIndex));
                }
            }

            if (!string.IsNullOrEmpty(str))
            {
                parts.Add(new Part(str, rowIndex, lines.Length - 1 - str.Length));
            }
        }

        return (parts, symbols);
    }

    private sealed record Part(string Value, int Row, int Column);

    private sealed record Symbol(char Value, int Row, int Column);
}
