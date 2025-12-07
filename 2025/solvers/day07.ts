const solvePart1 = (input: string) => {
    const rows = input.split('\n');
    const start = rows[0].indexOf('S');
    const splittersByRow = rows.map(row => allIndexesOf('^', row));

    let splits = 0;
    const beans = new Set<number>([start]);

    for (const splitters of splittersByRow) {
        for (const index of splitters) {
            if (beans.has(index)) {
                splits++;
                beans.add(index + 1);
                beans.add(index - 1);
                beans.delete(index);
            }
        }
    }

    return splits;
}

const solvePart2 = (input: string) => {
    const rows = input.split('\n');
    const start = rows[0].indexOf('S');
    const splittersByColumn = rows[0].split('').map((_, colIndex) =>
        rows.flatMap((row, rowIndex) => row[colIndex] === '^' ? [rowIndex] : [])
    );

    const cache: Map<string, number> = new Map();

    const solveFor = (pos: Pos): number => {
        const cacheKey = getCacheKey(pos);
        if (cache.has(cacheKey)) {
            return cache.get(cacheKey)!;
        }

        const splitterIndex = splittersByColumn[pos.x]
            .find(splitter => splitter > pos.y);

        if (!splitterIndex) {
            cache.set(cacheKey, 1);
            return 1;
        }

        const left = { x: pos.x - 1, y: splitterIndex + 1 };
        const right = { x: pos.x + 1, y: splitterIndex + 1 };

        const result = solveFor(left) + solveFor(right);
        cache.set(cacheKey, result);
        return result;
    }

    return solveFor({x: start, y: 0});
};

const allIndexesOf = (char: string, str: string) => [...str].map((_, i) => i).filter(i => str[i] === char);

const getCacheKey = (pos: Pos) => `${pos.x},${pos.y}`;

type Pos = { x: number, y: number };

export const day07 = {solvePart1, solvePart2};