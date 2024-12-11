namespace aoc2023.Solvers;

[Day(5)]
internal sealed class Day05 : IMrWolf
{
    public object SolvePart1(string input)
    {
        var parts = input.Split("\n\n");
        var seeds = GetSeeds(parts[0]);
        var maps = GetMaps(parts.Skip(1).ToArray());

        return seeds
            .Select(seed => MapSeeds(seed, maps))
            .Min();
    }

    public object SolvePart2(string input)
    {
        var parts = input.Split("\n\n");
        var ranges = GetRanges(parts[0]);
        var maps = GetMaps(parts.Skip(1).ToArray());

        return TransformRanges(ranges, maps)
            .MinBy(range => range.Start)!
            .Start;
    }

    private static List<Map> GetMaps(string[] source)
        => source
            .Select((src, index) => new Map
                (
                    index,
                    src
                        .Split('\n')
                        .Skip(1)
                        .Select(line => line.Split(' ').Select(long.Parse).ToList())
                        .Select(x => new Rule(x[0], x[1], x[2]))
                        .ToList()
                )
            )
            .ToList();

#region Part1

    private static IEnumerable<long> GetSeeds(string source)
        => source
            .Split(' ')
            .Skip(1)
            .Select(long.Parse);

    private static long MapSeeds(long seed, List<Map> maps)
    {
        foreach (var map in maps)
        {
            var rule = map.Rules.FirstOrDefault(r => r.CanTransform(seed));
            seed = rule?.Transform(seed) ?? seed;
        }

        return seed;
    }

#endregion

#region Part2

    private static List<Range> GetRanges(string source)
        => source
            .Split(' ')
            .Skip(1)
            .Select((src, index) => new {Index = index, Value = long.Parse(src)})
            .GroupBy(x => x.Index / 2, x => x.Value)
            .Select(g => g.ToArray())
            .Select(e => new Range(e[0], e[0] + e[1] - 1))
            .ToList();

    private static List<Range> TransformRanges(List<Range> ranges, List<Map> maps)
    {
        if (maps.Count == 0)
        {
            return ranges;
        }

        var map = maps.First();
        var mapped = new List<Range>();
        var notMapped = ranges;

        foreach (var rule in map.Rules)
        {
            var (transformed, notTransformed) = rule.Transform(notMapped);
            mapped.AddRange(transformed);
            notMapped = notTransformed;
        }

        mapped.AddRange(notMapped);
        return TransformRanges(mapped, maps.Skip(1).ToList());
    }

#endregion
}

internal sealed record Map(int Level, List<Rule> Rules);

internal sealed record Rule(long TargetStart, long Start, long Length)
{
    public long End => Start + Length - 1;

    public bool CanTransform(long seed) => seed >= Start && seed <= End;

    public long Transform(long seed) => TargetStart + (seed - Start);
}

internal sealed record Range(long Start, long End);

internal static class RuleExtensions
{
    public static (List<Range> transformed, List<Range> notTransformed) Transform(this Rule rule, List<Range> ranges)
    {
        var transformed = new List<Range>();
        var notTransformed = new List<Range>();

        foreach (var range in ranges)
        {
            // not overlapping
            // -----|-------|-------------- Rule
            // ---------------|----|------- Range
            if (range.Start > rule.End || rule.Start > range.End)
            {
                notTransformed.Add(range);
                continue;
            }

            // contained
            // -----|--------------|------- Rule
            // ---------|-----|------------ Range
            if (range.Start >= rule.Start && range.End <= rule.End)
            {
                transformed.Add(new Range
                (
                    rule.Transform(range.Start),
                    rule.Transform(range.End)
                ));

                continue;
            }

            // overflow both sides
            // ---------|-----|------------ Rule
            // -----|--------------|------- Range
            if (range.Start < rule.Start && range.End > rule.End)
            {
                notTransformed.Add(range with {End = rule.Start - 1});
                notTransformed.Add(range with {Start = rule.End + 1});

                transformed.Add(new Range
                (
                    rule.Transform(rule.Start),
                    rule.Transform(rule.End)
                ));

                continue;
            }

            // left overlap
            // ----------|---------|------- Rule
            // -----|---------|------------ Range
            if (range.Start < rule.Start && range.End >= rule.Start)
            {
                notTransformed.Add(range with {End = rule.Start - 1});

                transformed.Add(new Range
                (
                    rule.Transform(rule.Start),
                    rule.Transform(range.End)
                ));

                continue;
            }

            // right overlap
            // -----|--------|------------- Rule
            // ---------|----------|------- Range
            if (range.Start <= rule.End && range.End > rule.End)
            {
                notTransformed.Add(range with {Start = rule.End + 1});

                transformed.Add(new Range(
                    rule.Transform(range.Start),
                    rule.Transform(rule.End)
                ));
            }
        }

        return (transformed, notTransformed);
    }
}
