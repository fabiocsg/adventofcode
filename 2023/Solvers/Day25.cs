namespace aoc2023.Solvers;

[Day(25)]
internal sealed class Day25 : IMrWolf
{
    public object SolvePart1(string input)
    {
        var links = ParseInput(input);
        int cut, left, right;

        do
        {
            (cut, left, right) = Split(links);
        } while (cut != 3);

        return left * right;
    }

    public object SolvePart2(string input)
        => "🎄❄️🥂🎉🥳";

    // https://en.wikipedia.org/wiki/Karger%27s_algorithm
    private static (int cut, int left, int right) Split(Link[] links)
    {
        var nodeGroups = links
            .Select(l => l.From)
            .Distinct()
            .Select(node => new List<string> {node})
            .ToList();

        var rand = new Random();

        while (nodeGroups.Count > 2)
        {
            var link = links[rand.Next(links.Length)];
            var group1 = nodeGroups.First(g => g.Contains(link.From));
            var group2 = nodeGroups.First(g => g.Contains(link.To));

            if (group1 != group2)
            {
                nodeGroups.Remove(group2);
                group1.AddRange(group2);
            }
        }

        var count = Enumerable.Range(0, links.Length)
            .Select(i => links[i])
            .Count(link => nodeGroups.FindIndex(n => n.Contains(link.From)) != nodeGroups.FindIndex(n => n.Contains(link.To)));

        // the count includes both directions for every link => divide by 2
        return (count / 2, nodeGroups[0].Count, nodeGroups[1].Count);
    }

    private static Link[] ParseInput(string input)
        => input.Split('\n')
            .Select(l => l.Split(new[] {':', ' '}, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .SelectMany(parts => parts.Skip(1).SelectMany(node => new[] {new Link(parts[0], node), new Link(node, parts[0])}))
            .ToArray();

    private sealed record Link(string From, string To);
}
