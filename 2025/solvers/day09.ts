const solvePart1 = (input: string) => {
    const tiles = parseTiles(input);
    return findAllPairs(tiles)
        .map(pair => findArea(pair.t1, pair.t2))
        .reduce((max, value) => Math.max(max, value), 0);
}

const solvePart2 = (input: string) => {
    const tiles = parseTiles(input);
    const pairs = findAllPairs(tiles);
    const edges = tiles.map((t1, i) => ({t1, t2: tiles[(i + 1) % tiles.length]}));

    return pairs
        .filter((pair) => edges.every(edge => !pairContainsEdge(pair, edge)))
        .map(pair => findArea(pair.t1, pair.t2))
        .reduce((max, value) => Math.max(max, value), 0);
};

const parseTiles = (input: string): Tile[] =>
    input.split('\n').map(row => {
        const [x, y] = row.split(',').map(Number);
        return {x, y};
    });

const findAllPairs = (tiles: Tile[]) =>
    tiles.flatMap((t1, i) =>
        tiles
            .slice(i + 1)
            .map(t2 => ({t1, t2}))
    );

const findArea = (a: Tile, b: Tile) => {
    const dx = Math.abs(a.x - b.x) + 1;
    const dy = Math.abs(a.y - b.y) + 1;
    return dx * dy;
}

const pairContainsTile = (pair: Pair, tile: Tile) => {
    const x1 = Math.min(pair.t1.x, pair.t2.x);
    const x2 = Math.max(pair.t1.x, pair.t2.x);
    const y1 = Math.min(pair.t1.y, pair.t2.y);
    const y2 = Math.max(pair.t1.y, pair.t2.y);
    return x1 < tile.x
        && x2 > tile.x
        && y1 < tile.y
        && y2 > tile.y;
};

// this works because we know that edges are always horizontal or vertical
const pairContainsEdge = (pair: Pair, edge: Edge) => {
    const {t1, t2} = edge;
    const x = Math.floor((t1.x + t2.x) / 2);
    const y = Math.floor((t1.y + t2.y) / 2);
    const t3 = {x, y};
    return pairContainsTile(pair, t1)
        || pairContainsTile(edge, t2)
        || pairContainsTile(pair, t3);
};

type Tile = { x: number, y: number };
type Pair = { t1: Tile, t2: Tile };
type Edge = { t1: Tile, t2: Tile };

export const day09 = {solvePart1, solvePart2};