using System.Text.RegularExpressions;

namespace aoc2023.Solvers;

[Day(2)]
internal sealed class Day02 : IMrWolf
{
    public object SolvePart1(string input)
        => input.Split('\n')
            .Select(GetRecord)
            .Where(IsValid)
            .Sum(game => game.Id);

    public object SolvePart2(string input)
        => input.Split('\n')
            .Select(GetRecord)
            .Select(game => game.Red * game.Green * game.Blue)
            .Sum();

    private static GameRecord GetRecord(string line)
    {
        var id = int.Parse(line.Split(':')[0].Split(' ')[1]);
        var red = GetColor(line, "red");
        var green = GetColor(line, "green");
        var blue = GetColor(line, "blue");
        return new GameRecord(id, red, green, blue);
    }

    private static int GetColor(string source, string color)
        => Regex.Matches(source, $"(\\d+) {color}")
            .Select(x => int.Parse(x.Groups[1].Value))
            .Max();

    private static bool IsValid(GameRecord game) => game is
    {
        Red: <= 12,
        Green: <= 13,
        Blue: <= 14,
    };

    private sealed record GameRecord(int Id, int Red, int Green, int Blue);
}
