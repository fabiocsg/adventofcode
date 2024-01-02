namespace aoc2023.Solvers;

[Day(1)]
internal sealed class Day01 : IMrWolf
{
    private static readonly string[] Digits = {"1", "2", "3", "4", "5", "6", "7", "8", "9"};
    private static readonly string[] Words = {"one", "two", "three", "four", "five", "six", "seven", "eight", "nine"};

    public object SolvePart1(string input)
        => input.Split('\n')
            .Select(line => FindNumber(line))
            .Sum();

    public object SolvePart2(string input)
        => input.Split('\n')
            .Select(line => FindNumber(line, true))
            .Sum();

    private static int FindNumber(string source, bool useWords = false)
    {
        var searchFor = !useWords
            ? Digits
            : Digits.Union(Words).ToArray();

        var first = searchFor
            .Select(x => new {Value = x, Index = source.IndexOf(x, StringComparison.InvariantCultureIgnoreCase)})
            .Where(x => x.Index >= 0)
            .MinBy(x => x.Index)!;

        var last = searchFor
            .Select(x => new {Value = x, Index = source.LastIndexOf(x, StringComparison.InvariantCultureIgnoreCase)})
            .Where(x => x.Index >= 0)
            .MaxBy(x => x.Index)!;

        return int.Parse($"{ToDigit(first.Value)}{ToDigit(last.Value)}");
    }

    private static int ToDigit(string source) => source switch
    {
        "one" => 1,
        "two" => 2,
        "three" => 3,
        "four" => 4,
        "five" => 5,
        "six" => 6,
        "seven" => 7,
        "eight" => 8,
        "nine" => 9,
        _ => int.Parse(source),
    };
}
