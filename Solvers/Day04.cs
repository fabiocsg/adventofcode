using aoc2023.Core;

namespace aoc2023.Solvers;

[Day(4)]
internal sealed class Day04 : IMrWolf
{
    public object SolvePart1(string input)
        => input.Split('\n')
            .Select(Parse)
            .Where(card => card.WinCount > 0)
            .Sum(card => Math.Pow(2, card.WinCount - 1));

    public object SolvePart2(string input)
    {
        var cards = input.Split('\n').Select(Parse).ToList();
        var amounts = cards.ToDictionary(card => card.Id, _ => 1);

        // for every card
        for (var i = 0; i < cards.Count; i++)
        {
            var card = cards[i];

            // how many cards with this id I have
            var amount = amounts[card.Id];

            for (var j = 0; j < cards[i].WinCount; j++)
            {
                // id of the card I have copies of
                var id = 2 + i + j;

                if (id >= cards.Count)
                {
                    // "Cards will never make you copy a card past the end of the table", better safe than sorry.
                    break;
                }

                amounts[id] += amount;
            }
        }

        return amounts.Values.Sum();
    }

    private static Card Parse(string source)
    {
        var parts = source.Split(':', '|').ToArray();
        var id = int.Parse(parts[0].Split(' ').Last());
        var winningNumbers = GetNumbers(parts[1]);
        var myNumbers = GetNumbers(parts[2]);
        var count = myNumbers.Count(n => winningNumbers.Contains(n));
        return new Card(id, count);
    }

    private static List<int> GetNumbers(string source)
        => source.Split(' ')
            .Where(x => !string.IsNullOrEmpty(x))
            .Select(x => int.Parse(x.Trim()))
            .ToList();

    private sealed record Card(int Id, int WinCount);
}
