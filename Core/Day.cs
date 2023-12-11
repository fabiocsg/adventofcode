namespace aoc2023.Core;

[AttributeUsage(AttributeTargets.Class)]
internal sealed class Day : Attribute
{
    public Day(int dayNumber)
        => DayNumber = dayNumber;

    public int DayNumber { get; }
}
