const solvePart1 = (input: string) => {
    return input
        .split('\n')
        .reduce((dial, command) => {
            const rotation = getRotation(command);
            dial.position = mod(dial.position + rotation, 100);
            if (dial.position === 0) {
                dial.zeros++;
            }
            return dial;
        }, {position: 50, zeros: 0})
        .zeros;
};

const solvePart2 = (input: string) => {
    return input
        .split('\n')
        .reduce((dial, command) => {
            const rotation = getRotation(command);
            const prev = dial.position;
            const current = prev + (rotation % 100);
            dial.position = mod(current, 100);

            const fullRotations = Math.trunc(Math.abs(rotation) / 100);
            const crossedZero = prev !== 0 && (current <= 0 || current > 99);
            dial.zeros += fullRotations + (crossedZero ? 1 : 0);

            return dial;
        }, {position: 50, zeros: 0})
        .zeros;
};

const getRotation = (command: string) => command[0] === 'R'
    ? +command.slice(1)
    : -+command.slice(1);

const mod = (num: number, mod: number) => ((num % mod) + mod) % mod;

export const day01 = {solvePart1, solvePart2};