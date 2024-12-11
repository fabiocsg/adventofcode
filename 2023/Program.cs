using System.Diagnostics;
using System.Reflection;

namespace aoc2023;

internal class Program
{
    public static void Main(string[] args)
    {
        if (args.Length != 2 || !int.TryParse(args[0], out var day) || !File.Exists(args[1]))
        {
            Console.WriteLine("Invalid Arguments. Usage: {day:int} {inputFilePath:string}");
            return;
        }

        var input = File.ReadAllText(args[1]);

        var resolverType = typeof(Program).Assembly
            .GetTypes()
            .FirstOrDefault(t =>
                t.IsClass
                && typeof(IMrWolf).IsAssignableFrom(t)
                && t.GetCustomAttribute<Day>()?.DayNumber == day
            );

        if (resolverType == null)
        {
            Console.WriteLine("Not there yet ;)");
            return;
        }

        var resolver = (IMrWolf) Activator.CreateInstance(resolverType)!;
        SolveWithExecutionTime(() => resolver.SolvePart1(input), "Part 1");
        SolveWithExecutionTime(() => resolver.SolvePart2(input), "Part 2");
    }

    private static void SolveWithExecutionTime(Func<object> func, string name)
    {
        var sw = Stopwatch.StartNew();
        var result = func.Invoke();
        sw.Stop();
        Console.WriteLine($"{name}: {result}");
        Console.WriteLine($"Solved in {sw.Elapsed.TotalSeconds}s");
        Console.WriteLine();
    }
}
