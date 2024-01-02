namespace aoc2023.Solvers;

[Day(13)]
internal sealed class Day13 : IMrWolf
{
    public object SolvePart1(string input)
        => Solve(input, 0);

    public object SolvePart2(string input)
        => Solve(input, 1);

    private static int Solve(string input, int smudges)
        => input.Split("\n\n")
            .Select(pattern => SolvePattern(pattern, smudges))
            .Sum();

    private static int SolvePattern(string pattern, int smudges)
    {
        var entries = pattern.Split('\n');

        var horizontal = FindReflection(entries, smudges);

        if (horizontal >= 0)
        {
            return 100 * (horizontal + 1);
        }

        entries = Rotate(entries);
        var vertical = FindReflection(entries, smudges);

        if (vertical >= 0)
        {
            return vertical + 1;
        }

        throw new ShouldNeverHappenException();
    }

    private static int FindReflection(string[] entries, int smudges)
    {
        foreach (var index in Enumerable.Range(0, entries.Length - 1))
        {
            if (Reflects(entries, index, smudges))
            {
                return index;
            }
        }

        return -1;
    }

    private static bool Reflects(string[] entries, int start, int smudges)
    {
        var totalDiffs = 0;

        for (var i = 0;; i++)
        {
            var a = start - i;
            var b = start + i + 1;

            if (a < 0 || b >= entries.Length)
            {
                return totalDiffs == smudges;
            }

            totalDiffs += CountDiff(entries[a], entries[b]);

            if (totalDiffs > smudges)
            {
                return false;
            }
        }
    }

    private static string[] Rotate(string[] entries)
        => entries
            .SelectMany(entry => entry.Select((ch, i) => new {Char = ch, Index = i}))
            .GroupBy(x => x.Index)
            .Select(group => new string(group.Select(x => x.Char).Reverse().ToArray()))
            .ToArray();

    private static int CountDiff(string a, string b)
        => Enumerable
            .Range(0, a.Length)
            .Select(x => a[x] != b[x])
            .Count(x => x);
}
