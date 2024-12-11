namespace aoc2023.Solvers;

[Day(9)]
internal sealed class Day09 : IMrWolf
{
    public object SolvePart1(string input)
        => Solve(input, FindNext);

    public object SolvePart2(string input)
        => Solve(input, FindPrevious);

    private static long Solve(string input, Func<long[], long> findDelegate)
        => input.Split('\n')
            .Select(line => line.Split(' ').Select(long.Parse).ToArray())
            .Select(findDelegate)
            .Sum();

    private static long FindNext(long[] source)
    {
        var steps = new long[source.Length - 1];

        for (var i = 1; i < source.Length; i++)
        {
            steps[i - 1] = source[i] - source[i - 1];
        }

        return steps.Distinct().Count() == 1
            ? source[^1] + steps[0]
            : source[^1] + FindNext(steps);
    }

    private static long FindPrevious(long[] source)
        => FindNext(source.Reverse().ToArray());
}
