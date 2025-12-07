import {ShouldNeverHappenError} from "../core.ts";

const solvePart1 = (input: string) => {
    const rows = input
        .split('\n')
        .map(row => row.split(' ').filter(e => e.trim() != ''));

    return transpose(rows)
        .map(solveProblem)
        .reduce((acc, value) => acc + value, 0)
}

const solvePart2 = (input: string) => {
    const rows = input.split('\n')
        .map(row => row.split(''));

   return rotateCounterClockwise(rows)
        .map(row => row.join('').trim())
        .reduce<string[][]>((acc, item) => {
            if (!item) {
                return [...acc, []];
            }
            if (item.endsWith('+') || item.endsWith('*')) {
                acc[acc.length - 1].push(item.slice(0, -1).trim());
                acc[acc.length - 1].push(item.at(-1)!);
            } else {
                acc[acc.length - 1].push(item);
            }
            return acc;
        }, [[]])
        .map(solveProblem)
        .reduce((acc, value) => acc + value, 0);
};

const solveProblem = (problem: string[]) => {
    const operation = problem.at(-1)!;
    return problem.slice(0, -1).map(Number).reduce((a, b) => compute(+a, +b, operation));
}

const compute = (a: number, b: number, operation: string) => {
    switch (operation) {
        case '+': return a + b;
        case '*': return a * b;
        default: throw new ShouldNeverHappenError();
    }
}

const transpose = (matrix: string[][]) =>
    matrix[0].map((_, colIndex) => matrix.map(row => row[colIndex]));

const rotateCounterClockwise = (matrix: string[][]) =>
    transpose(matrix).reverse();

export const day06 = {solvePart1, solvePart2};