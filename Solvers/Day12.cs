namespace aoc2023.Solvers;

[Day(12)]
internal sealed class Day12 : IMrWolf
{
    private readonly Dictionary<Key, long> _store = new();

    public object SolvePart1(string input)
        => Solve(input, false);

    public object SolvePart2(string input)
        => Solve(input, true);

    private long Solve(string input, bool unfold)
        => input.Split('\n')
            .Select(x => ParseRow(x, unfold))
            .Select(x => Scan(x.record, x.groups))
            .Sum();

    private static (string record, int[] groups) ParseRow(string row, bool unfold)
    {
        var parts = row.Split(' ');
        var times = unfold ? 5 : 1;
        var record = Repeat(parts[0], '?', times);
        var groups = Repeat(parts[1], ',', times);
        return (record, groups.Split(',').Select(int.Parse).ToArray());
    }

    private static string Repeat(string source, char separator, int times)
        => string.Join(separator, Enumerable.Repeat(source, times));

    private long Scan(string record, int[] groups)
    {
        var key = new Key(record, string.Join(',', groups));

        if (_store.TryGetValue(key, out var value))
        {
            return value;
        }

        var result = record switch
        {
            "" when groups.Length == 0 => 1,
            "" when groups.Length > 0 => 0,
            _ when record[0] == '.' => Scan(record[1..], groups),
            _ when record[0] == '?' => Scan($"#{record[1..]}", groups) + Scan(record[1..], groups),
            _ when record[0] == '#' => Check(record, groups),
            _ => throw new ShouldNeverHappenException(),
        };

        _store.Add(key, result);
        return result;
    }

    private long Check(string record, int[] groups)
    {
        if (groups.Length == 0)
        {
            return 0;
        }

        var available = record.Split('.')[0].Length;
        var scanLength = groups[0];

        if (available < scanLength)
        {
            // not enough # or ? to satisfy the current group
            return 0;
        }

        if (available == scanLength && record.Length == scanLength)
        {
            return Scan(string.Empty, groups[1..]);
        }

        if (record[scanLength] == '#')
        {
            // took me way too long to figure this out: when there's a match the next
            // entry MUST be a . or a ? in order to space the broken group from the next one
            return 0;
        }

        var takeFrom = scanLength + 1;
        return Scan(record[takeFrom..], groups[1..]);
    }

    private sealed record Key(string Record, string Groups);
}
