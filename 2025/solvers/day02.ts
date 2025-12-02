const solvePart1 = (input: string) => solve(input, repeatsOnce)

const solvePart2 = (input: string) => solve(input, hasRepeatingPattern);

const solve = (input: string, filterPredicate: (value: string) => boolean) => {
    return input
        .split(',')
        .flatMap(getRange)
        .filter(n => filterPredicate(n.toString()))
        .reduce((acc, value) => acc + value, 0);
};

const getRange = (range: string) => {
    const [min, max] = range.split('-').map(Number);
    return [...Array(max - min + 1)].map((_, i) => min + i);
}

const repeatsOnce = (value: string) => repeats(value, 2);

const hasRepeatingPattern = (value: string) => {
    for (let i = 2; i <= value.length; i++) {
        if (repeats(value, i)){
            return true;
        }
    }

    return false;
}

const repeats = (value: string, parts: number) => {
    if (value.length % parts !== 0) {
        return false;
    }
    const chunk = value.slice(0, value.length / parts);
    return chunk.repeat(parts) === value;
}

export const day02 = {solvePart1, solvePart2};