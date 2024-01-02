namespace aoc2023.Solvers;

[Day(7)]
internal sealed class Day07 : IMrWolf
{
    public object SolvePart1(string input)
        => Solve(input);

    public object SolvePart2(string input)
        => Solve(input, true);

    private static int Solve(string input, bool useJokers = false)
        => ParseInput(input)
            .OrderBy(hand => hand.GetHandType(useJokers))
            .ThenBy(hand => CardValue(hand.Cards[0], useJokers))
            .ThenBy(hand => CardValue(hand.Cards[1], useJokers))
            .ThenBy(hand => CardValue(hand.Cards[2], useJokers))
            .ThenBy(hand => CardValue(hand.Cards[3], useJokers))
            .ThenBy(hand => CardValue(hand.Cards[4], useJokers))
            .Select((hand, index) => hand.Bid * (index + 1))
            .Sum();

    private static IEnumerable<Hand> ParseInput(string input)
        => input.Split('\n')
            .Select(ParseHand);

    private static Hand ParseHand(string source)
    {
        var parts = source.Split(' ');
        return new Hand(parts[0], int.Parse(parts[1]));
    }

    private static int CardValue(char card, bool usejoker = false)
        => card switch
        {
            '2' => 2,
            '3' => 3,
            '4' => 4,
            '5' => 5,
            '6' => 6,
            '7' => 7,
            '8' => 8,
            '9' => 9,
            'T' => 10,
            'J' when !usejoker => 11,
            'J' when usejoker => 1,
            'Q' => 12,
            'K' => 13,
            'A' => 14,
            _ => throw new ArgumentOutOfRangeException(nameof(card)),
        };

    private sealed record Hand(string Cards, int Bid)
    {
        public HandType GetHandType(bool useJoker = false)
        {
            var dict = Cards
                .GroupBy(card => card)
                .ToDictionary(
                    group => group.Key,
                    group => group.ToList()
                );

            if (useJoker && dict.TryGetValue('J', out var jokers) && jokers.Count != 5)
            {
                dict.Remove('J');
                dict.MaxBy(x => x.Value.Count).Value.AddRange(jokers);
            }

            var maxCount = dict.Values.Max(g => g.Count);

            return dict.Count switch
            {
                5 => HandType.HighCard,
                4 => HandType.OnePair,
                3 when maxCount is 2 => HandType.TwoPairs,
                3 when maxCount is 3 => HandType.ThreeOfAKind,
                2 when maxCount is 3 => HandType.FullHouse,
                2 when maxCount is 4 => HandType.FourOfAKind,
                1 => HandType.FiveOfAKind,
                _ => throw new ArgumentOutOfRangeException(nameof(dict.Count)),
            };
        }
    }

    private enum HandType
    {
        HighCard = 0,
        OnePair = 1,
        TwoPairs = 2,
        ThreeOfAKind = 3,
        FullHouse = 4,
        FourOfAKind = 5,
        FiveOfAKind = 6,
    }
}
