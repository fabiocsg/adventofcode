const solvePart1 = (input: string) => solvePath(['you', 'out'], parseCircuit(input));

const solvePart2 = (input: string) => {
    const circuit = parseCircuit(input);
    return solvePath(['svr', 'fft', 'dac', 'out'], circuit)
        + solvePath(['svr', 'dac', 'fft', 'out'], circuit);
}

const solvePath = (nodes: string[], circuit: Record<string, string[]>) =>
    nodes.slice(0, -1)
        .map((value, i) => ({from: value, to: nodes[i + 1]}))
        .reduce((acc, {from, to}) => acc * countPathsFromTo(from, to, circuit, new Map()), 1);

const countPathsFromTo = (from: string, to: string, circuit: Record<string, string[]>, memo: Map<string, number>): number => {
    if (memo.has(from)) {
        return memo.get(from)!;
    }

    if (from === to) {
        return 1;
    }

    if (!circuit[from]) {
        return 0
    }

    const count = circuit[from]
        .map(next => countPathsFromTo(next, to, circuit, memo))
        .reduce((acc, value) => acc + value, 0);

    memo.set(from, count);
    return count;
}

const parseCircuit = (source: string): Record<string, string[]> =>
    source.split('\n')
        .map(parseDevice)
        .reduce((acc, dev) => {
            acc[dev.input] = dev.output;
            return acc;
        }, {} as Record<string, string[]>)

const parseDevice = (source: string): Device => {
    const [i, o] = source.split(':').map(s => s.trim());
    return {input: i, output: o.split(' ')};
}

type Device = { input: string, output: string[] };

export const day11 = {solvePart1, solvePart2};