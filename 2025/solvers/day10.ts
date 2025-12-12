const solvePart1 = (input: string) => solveWithPredicate(input, solveDiagram);

const solvePart2 = (input: string) => solveWithPredicate(input, solveJoltage);

const solveWithPredicate = (input: string, predicate: (machine: Machine) => number) =>
    parseInput(input)
        .map(predicate)
        .reduce((acc, value) => acc + value, 0);

const solveDiagram = (machine: Machine): number => {
    const visited = new Set<number>([0]);
    const masks = machine.buttons.map((button: number[]) =>
        button.reduce((mask, bit) => mask | (1 << bit), 0));

    const queue = [{state: 0, presses: 0}];

    while (true) {
        const item = queue.shift()!;

        for (const mask of masks) {
            const next = item.state ^ mask;
            if (visited.has(next)) {
                continue;
            }

            if (next === machine.diagram) {
                return item.presses + 1;
            }

            visited.add(next);
            queue.push({state: next, presses: item.presses + 1});
        }
    }
}

// this probably should have been an ILP, but I already spent too much time on it, and even if
// it's slower than what I usually aim for there's no way I'm starting from scratch again...
const solveJoltage = (machine: Machine): number => {
    const availableButtonsMask = (1 << machine.buttons.length) - 1;
    return findMinPresses(machine.joltage, machine.buttons, availableButtonsMask);
};

const findMinPresses = (state: number[], buttons: Button[], enabledButtonsMask: number) => {

    if (state.every(x => x === 0)) {
        return 0;
    }

    const targetIndex = getNextTargetIndex(state, buttons, enabledButtonsMask);
    if (targetIndex < 0) {
        return Infinity; // unsolvable
    }

    const expectedButtonPresses = state[targetIndex];
    const relevantButtonsIndexes = buttons
        .map((_, i) => i)
        .filter(i => buttons[i].includes(targetIndex) && isButtonEnabled(i, enabledButtonsMask));

    if (!relevantButtonsIndexes.length) {
        return Infinity; // unsolvable
    }

    const disabledButtonsMask = relevantButtonsIndexes.reduce((acc, btnIndex) => acc | (1 << btnIndex), 0);
    const nextEnableButtonsMask = enabledButtonsMask & ~disabledButtonsMask;

    const buttonsPattern = new Array<number>(relevantButtonsIndexes.length).fill(0);
    buttonsPattern[relevantButtonsIndexes.length - 1] = expectedButtonPresses;

    let result = Infinity;

    do {
        const nextState = [...state];
        let isValidNextState = true;
        for (let i = 0; i < buttonsPattern.length; i++) {
            const button = buttons[relevantButtonsIndexes[i]];
            const numberOfPresses = buttonsPattern[i];
            for (const affectedIndex of button) {
                nextState[affectedIndex] -= numberOfPresses;
                if (nextState[affectedIndex] < 0) {
                    isValidNextState = false;
                    break;
                }
            }

            if (!isValidNextState) {
                break;
            }
        }

        if (!isValidNextState) {
            continue;
        }

        result = Math.min(result, expectedButtonPresses + findMinPresses(nextState, buttons, nextEnableButtonsMask));

    } while (iteratePossibleButtonPatterns(buttonsPattern));

    return result;
}

// find the index of state having in order:
// 1. the min number of buttons targeting it
// 2. the highest value of the joltage index
const getNextTargetIndex = (state: number[], buttons: Button[], enabledButtonsMask: number) => {
    let targetIndex = -1;
    let targetButtonsCount = Infinity;

    for (let i = 0; i < state.length; i++) {
        const pressesLeft = state[i];
        if (pressesLeft === 0) {
            continue;
        }

        const indexButtonsCount = buttons
            .filter((btn, btnIndex) => btn.includes(i) && isButtonEnabled(btnIndex, enabledButtonsMask))
            .length;

        if (indexButtonsCount < targetButtonsCount || (indexButtonsCount === targetButtonsCount && state[i] > state[targetIndex])) {
            targetIndex = i;
            targetButtonsCount = indexButtonsCount;
        }
    }

    return targetIndex;
}

const isButtonEnabled = (buttonIndex: number, enabledButtonsMask: number) =>
    (enabledButtonsMask & (1 << buttonIndex)) > 0;

// generate the next possible combination of buttons presses
const iteratePossibleButtonPatterns = (buttonPressesByIndex: number[]) => {
    let leftMostNonZeroIndex = -1;
    for (let i = buttonPressesByIndex.length - 1; i >= 0; i--) {
        if (buttonPressesByIndex[i] !== 0) {
            leftMostNonZeroIndex = i;
            break;
        }
    }

    if (leftMostNonZeroIndex <= 0) {
        return false;
    }

    const valueToMove = buttonPressesByIndex[leftMostNonZeroIndex];
    buttonPressesByIndex[leftMostNonZeroIndex - 1] += 1;
    buttonPressesByIndex[leftMostNonZeroIndex] = 0;
    buttonPressesByIndex[buttonPressesByIndex.length - 1] = valueToMove - 1;

    return true;
}

const parseInput = (input: string) => input.split('\n').map(parseMachine);

const parseMachine = (source: string): Machine => {
    const parts = source.split(' ');
    const diagram = parseBinaryDiagram(parts.at(0)!);
    const buttons = parseButtons(parts.slice(1, -1));
    const joltage = parseJoltage(parts.at(-1)!);
    return {diagram, buttons, joltage};
}

const parseBinaryDiagram = (source: string) =>
    [...(source.slice(1, -1))].reverse().reduce((acc, ch) => (acc << 1) | (ch === '#' ? 1 : 0), 0);

const parseButtons = (source: string[]) =>
    source.map(x => x.slice(1, -1).split(',').map(Number));

const parseJoltage = (source: string) =>
    source.slice(1, -1).split(',').map(Number);

type Machine = { diagram: number, buttons: Button[], joltage: number[] };
type Button = number[];

export const day10 = {solvePart1, solvePart2};