const solvePart1 = (input: string) => {
    const parts = input.split('\n\n');
    const shapes = parts.slice(0, -1).map(parseShape);
    const regions = parts.at(-1)?.split('\n').map(parseRegion)!;

    return regions.filter(region => fitsRegion(region, shapes)).length;
};

const solvePart2 = (_: string) => '🎄❄️🥂🎉🥳';

const fitsRegion = (region: Region, shapes: number[]) => {
    const {x, y, amounts} = region;
    const regionSize = x * y;
    const shapesSize = amounts
        .map((amount, index) => amount * shapes[index])
        .reduce((acc, value) => acc + value, 0);
    return shapesSize <= regionSize;
}

const parseShape = (input: string) => input.split('').filter(ch => ch === '#').length

const parseRegion = (input: string): Region => {
    const [size, amounts] = input.split(':');
    const [x, y] = size.split('x').map(Number);
    return {x, y, amounts: amounts.trim().split(' ').map(Number)};
}

type Region = { x: number, y: number, amounts: number[] };

export const day12 = {solvePart1, solvePart2};