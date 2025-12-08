const solvePart1 = (input: string) => {
    const junctions = input.split('\n').map(parseJunction);
    const {treeNodes, sizes, distances, findRoot} = initJunctionsTree(junctions);

    for (let i = 0; i < 1000; i++) {
        const {j1, j2} = distances[i];
        const root1 = findRoot(j1.key);
        const root2 = findRoot(j2.key);
        if (root1 === root2) {
            continue;
        }

        treeNodes[root2] = root1;
        sizes[root1] += sizes[root2];
        delete sizes[root2];
    }

    return Object.values(sizes)
        .sort((a, b) => b - a)
        .slice(0, 3)
        .reduce((acc, size) => acc * size, 1);
}

const solvePart2 = (input: string) => {
    const junctions = input.split('\n').map(parseJunction);
    const {treeNodes, sizes, distances, findRoot} = initJunctionsTree(junctions);

    for (let i = 0;; i++) {
        const {j1, j2} = distances[i];
        const root1 = findRoot(j1.key);
        const root2 = findRoot(j2.key);
        if (root1 === root2) {
            continue;
        }

        treeNodes[root2] = root1;
        sizes[root1] += sizes[root2];
        delete sizes[root2];

        if (Object.keys(sizes).length === 1) {
            return j1.x * j2.x;
        }
    }
};

const parseJunction = (source: string): JunctionWithKey => {
    const [x, y, z] = source.split(',').map(Number);
    return {x, y, z, key: `${x},${y},${z}`};
}

const initJunctionsTree = (junctions: JunctionWithKey[]) => {
    const treeNodes: Record<string, string> = {};
    const sizes: Record<string, number> = {};

    for (const junction of junctions) {
        treeNodes[junction.key] = junction.key;
        sizes[junction.key] = 1;
    }

    const distances = junctions.flatMap((j1, i) =>
        junctions.slice(i + 1)
            .map((j2) => ({distance: findDistance(j1, j2), j1, j2}))
    ).sort((a, b) => a.distance - b.distance);

    const findRoot = (junction: string): string => {
        const root = treeNodes[junction];
        return root === junction ? root : findRoot(root);
    }

    return {treeNodes, sizes, distances, findRoot};
}

const findDistance = (a: Junction, b: Junction) => {
    const dx = a.x - b.x;
    const dy = a.y - b.y;
    const dz = a.z - b.z;
    return dx * dx + dy * dy + dz * dz;
}

type Junction = { x: number, y: number, z: number };
type JunctionWithKey = Junction & { key: string };

export const day08 = {solvePart1, solvePart2};