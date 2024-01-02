namespace aoc2023;

// "I solve problems." "Good, we got one." "So I heard."
internal interface IMrWolf
{
    object SolvePart1(string input);
    object SolvePart2(string input);
}

[AttributeUsage(AttributeTargets.Class)]
internal sealed class Day : Attribute
{
    public Day(int dayNumber)
        => DayNumber = dayNumber;

    public int DayNumber { get; }
}

public class ShouldNeverHappenException : Exception { }
