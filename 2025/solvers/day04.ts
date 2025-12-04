const solvePart1 = (input: string) => {
    const grid = parseGrid(input);
    return getAvailableRolls(grid);
};

const solvePart2 = (input: string) => {
    const grid = parseGrid(input);
    let count = 0;
    while (true) {
        const prev = count;
        count += getAvailableRolls(grid);
        if (prev === count) {
            break;
        }
    }
    return count;
};

const parseGrid = (input: string) => input.split('\n').map(row => row.split(''));

const getAvailableRolls = (grid: string[][]) => {
    const availableRolls: Pos[] = [];

    for (let y = 0; y < grid.length; y++) {
        for (let x = 0; x < grid[y].length; x++) {
            const pos: Pos = {x, y};
            if (containsRoll(pos, grid) && canBeAccessed(pos, grid)) {
                availableRolls.push(pos);
            }
        }
    }

    availableRolls.forEach(pos => grid[pos.y][pos.x] = '.');
    return availableRolls.length;
}

const canBeAccessed = (pos: Pos, grid: string[][]) =>
    directions
        .map(dir => dir(pos))
        .filter(pos => containsRoll(pos, grid))
        .length < 4;

const containsRoll = (pos: Pos, grid: string[][]) => grid[pos.y]?.[pos.x] === '@';

const up = (pos: Pos) => ({x: pos.x, y: pos.y - 1});
const down = (pos: Pos) => ({x: pos.x, y: pos.y + 1});
const left = (pos: Pos) => ({x: pos.x - 1, y: pos.y});
const right = (pos: Pos) => ({x: pos.x + 1, y: pos.y});
const upLeft = (pos: Pos) => ({x: pos.x - 1, y: pos.y - 1});
const upRight = (pos: Pos) => ({x: pos.x + 1, y: pos.y - 1});
const downLeft = (pos: Pos) => ({x: pos.x - 1, y: pos.y + 1});
const downRight = (pos: Pos) => ({x: pos.x + 1, y: pos.y + 1});

const directions = [up, down, left, right, upLeft, upRight, downLeft, downRight];

type Pos = { x: number, y: number };

export const day04 = {solvePart1, solvePart2};