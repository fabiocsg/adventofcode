const solvePart1 = (input: string) => {
    const [rangesString, productString] = input.split('\n\n');
    const ranges = parseAndNormalizeRanges(rangesString);

    return productString.split('\n')
        .reduce((acc, value) => isFresh(+value, ranges) ? acc + 1 : acc, 0);
};

const solvePart2 = (input: string) => {
    const [rangesString, _] = input.split('\n\n');
    return parseAndNormalizeRanges(rangesString)
        .reduce((acc, range) => acc + (range.upper - range.lower + 1), 0);
};

const parseAndNormalizeRanges = (rangesString: string) =>
    rangesString.split('\n')
        .map(parseRange)
        .sort((a, b) => a.lower - b.lower)
        .reduce((acc, range) => {
            const last = acc[acc.length - 1];
            if (last && last.upper >= range.lower - 1) {
                last.upper = Math.max(last.upper, range.upper);
            } else {
                acc.push(range);
            }
            return acc;
        }, [] as Range[])

const parseRange = (range: string): Range => {
    const [lower, upper] = range.split('-').map(Number);
    return {lower, upper};
};

const isFresh = (product: number, ranges: Range[]) => {
    let start = 0;
    let end = ranges.length - 1;
    while (start <= end) {
        const mid = Math.floor((start + end) / 2);
        const range = ranges[mid];
        if (product < range.lower) {
            end = mid - 1;
        } else if (product > range.upper){
            start = mid + 1;
        } else {
            return true;
        }
    }
    return false;
};

export type Range = { lower: number, upper: number };

export const day05 = {solvePart1, solvePart2};