using aoc2023.Core;

namespace aoc2023.Solvers;

[Day(20)]
internal sealed class Day20 : IMrWolf
{
    public object SolvePart1(string input)
    {
        var network = ParseNetwork(input);

        var allPulses = Enumerable.Range(0, 1000)
            .SelectMany(_ => Start(network))
            .ToList();

        var highs = allPulses.Count(p => p.Value == PulseValue.High);
        return highs * (allPulses.Count - highs);
    }

    public object SolvePart2(string input)
    {
        var network = ParseNetwork(input);

        // like in Day8 I can't think of any generic solution for this problem...
        // this solution only works because of the structure of the given input, which
        // has a single conjunction module connected to rx. to solve the problem
        // I'm checking the inputs on the conjunction module to find the pattern 
        // that send a low pulse to rx using LCM (same as Day8)

        var conjunction = network[network["rx"].Inputs[0]];
        var moduleCycles = new Dictionary<string, long>();

        for (var i = 1;; i++)
        {
            foreach (var pulse in Start(network))
            {
                if (pulse.Value == PulseValue.High && conjunction.Inputs.Contains(pulse.From) && pulse.To == conjunction.Name)
                {
                    moduleCycles.TryAdd(pulse.From, i);
                }
            }

            if (moduleCycles.Count == conjunction.Inputs.Length)
            {
                break;
            }
        }

        return moduleCycles
            .Select(x => x.Value)
            .Aggregate(Lcm);
    }

    private static IEnumerable<Pulse> Start(Dictionary<string, Module> network)
    {
        var queue = new Queue<Pulse>();
        queue.Enqueue(new Pulse("_", "broadcaster", PulseValue.Low));

        while (queue.TryDequeue(out var pulse))
        {
            yield return pulse;

            foreach (var sentPulse in network[pulse.To].Receive(pulse))
            {
                queue.Enqueue(sentPulse);
            }
        }
    }

    private static Dictionary<string, Module> ParseNetwork(string source)
    {
        var moduleOutputs = source
            .Replace("broadcaster", "_broadcaster")
            .Split('\n')
            .Select(x => new {Type = x[0], Modules = x[1..].Split(new[] {"->", ","}, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)})
            .Select(x => new {x.Type, Name = x.Modules[0], Outputs = x.Modules[1..]})
            .ToArray();

        var moduleInputs = moduleOutputs
            .ToDictionary(
                m => m.Name,
                x => moduleOutputs
                    .Where(m => m.Outputs.Contains(x.Name))
                    .Select(m => m.Name)
                    .ToArray()
            );

        var network = moduleOutputs
            .ToDictionary(
                m => m.Name,
                m => m.Type switch
                {
                    '%' => new FlipFlop(m.Name, moduleInputs[m.Name], m.Outputs) as Module,
                    '&' => new Conjunction(m.Name, moduleInputs[m.Name], m.Outputs),
                    '_' => new Broadcaster(m.Name, moduleInputs[m.Name], m.Outputs),
                    _ => throw new ShouldNeverHappenException(),
                }
            );

        network.Add("rx", new Broadcaster("rx", new[] {moduleOutputs.Single(m => m.Outputs.Contains("rx")).Name}, Array.Empty<string>()));
        return network;
    }

    // https://en.wikipedia.org/wiki/Least_common_multiple#Calculation
    private static long Lcm(long n1, long n2)
        => n1 * n2 / Gcd(n1, n2);

    // https://en.wikipedia.org/wiki/Euclidean_algorithm#Implementations
    private static long Gcd(long n1, long n2)
        => n2 == 0 ? n1 : Gcd(n2, n1 % n2);

    private abstract class Module
    {
        public string Name { get; }
        public string[] Inputs { get; }
        protected string[] Outputs { get; }

        protected Module(string name, string[] inputs, string[] outputs)
        {
            Name = name;
            Inputs = inputs;
            Outputs = outputs;
        }

        public abstract IEnumerable<Pulse> Receive(Pulse pulse);
    }

    private sealed class FlipFlop : Module
    {
        private bool _isOn;

        public FlipFlop(string name, string[] inputs, string[] outputs)
            : base(name, inputs, outputs) { }

        public override IEnumerable<Pulse> Receive(Pulse pulse)
        {
            if (pulse.Value == PulseValue.High)
            {
                return Enumerable.Empty<Pulse>();
            }

            _isOn = !_isOn;
            return Outputs.Select(output => new Pulse(Name, output, _isOn ? PulseValue.High : PulseValue.Low));
        }
    }

    private sealed class Conjunction : Module
    {
        private readonly Dictionary<string, PulseValue> _inputValues;

        public Conjunction(string name, string[] inputs, string[] outputs)
            : base(name, inputs, outputs)
        {
            _inputValues = inputs.ToDictionary(x => x, _ => PulseValue.Low);
        }

        public override IEnumerable<Pulse> Receive(Pulse pulse)
        {
            _inputValues[pulse.From] = pulse.Value;
            var value = _inputValues.Values.All(x => x == PulseValue.High) ? PulseValue.Low : PulseValue.High;
            return Outputs.Select(output => new Pulse(Name, output, value));
        }
    }

    private sealed class Broadcaster : Module
    {
        public Broadcaster(string name, string[] inputs, string[] outputs)
            : base(name, inputs, outputs) { }

        public override IEnumerable<Pulse> Receive(Pulse pulse)
            => Outputs.Select(output => new Pulse(Name, output, pulse.Value));
    }

    private sealed record Pulse(string From, string To, PulseValue Value);

    private enum PulseValue
    {
        Low,
        High,
    }
}
