const solvePart1 = (input: string) => solve(input, 2);

const solvePart2 = (input: string) => solve(input, 12);

const solve = (input: string, digits: number) =>
    input.split('\n')
        .map(row => findJoltage(row, digits))
        .map(Number)
        .reduce((acc, value) => acc + value, 0);

const findJoltage = (bank: string, digits: number) => {
    let result = '';
    let start = 0;
    for (let i = 0; i < digits; i++) {
        const end = bank.length - (digits - i - 1);
        const sectionToEvaluate = bank.slice(start, end);
        const digitIndex = indexOfHighestDigit(sectionToEvaluate);
        result += sectionToEvaluate[digitIndex];
        start += digitIndex + 1;
    }
    return result;
};

const indexOfHighestDigit = (value: string) =>
    [...value].reduce((best, ch, i, arr) => ch > arr[best] ? i : best, 0);

export const day03 = {solvePart1, solvePart2};