namespace aoc2023.Solvers;

[Day(15)]
internal sealed class Day15 : IMrWolf
{
    public object SolvePart1(string input)
        => Parse(input)
            .Select(Hash)
            .Sum();

    public object SolvePart2(string input)
    {
        var boxes = Enumerable.Range(0, 256)
            .ToDictionary(x => x, _ => new List<Lens>());

        var lenses = Parse(input)
            .Select(ParseLens)
            .ToList();

        foreach (var lens in lenses)
        {
            var hash = Hash(lens.Label);

            if (lens.FocalLength == null)
            {
                boxes[hash].RemoveAll(x => x.Label == lens.Label);
                continue;
            }

            var match = boxes[hash].FirstOrDefault(l => l.Label == lens.Label);

            if (match != null)
            {
                match.FocalLength = lens.FocalLength;
            }
            else
            {
                boxes[hash].Add(lens);
            }
        }

        return boxes
            .SelectMany(x => x.Value.Select((l, i) => (x.Key + 1) * (i + 1) * int.Parse(l.FocalLength!)))
            .Sum();
    }

    private static string[] Parse(string input)
        => input.Replace("\n", string.Empty)
            .Split(',');

    private static Lens ParseLens(string source)
    {
        var parts = source.Split(new[] {'-', '='}, StringSplitOptions.RemoveEmptyEntries);

        return new Lens
        {
            Label = parts[0],
            FocalLength = parts.Length == 2
                ? parts[1]
                : null,
        };
    }

    private static int Hash(string source)
        => source.Aggregate(0, (a, b) => (a + b) * 17 % 256);

    private sealed class Lens
    {
        public string Label { get; init; } = default!;
        public string? FocalLength { get; set; }
    }
}
