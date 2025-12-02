import * as fs from "node:fs";

import type {MrWolf} from "./core.ts";
import {day01} from "./solvers/day01.ts";
import {day02} from "./solvers/day02.ts";


const solvers: Record<number, MrWolf> = {
    1: day01,
    2: day02,
};

const getSolver = (day: number) => solvers[day] || null;

const solveWithExecutionTime = (name: string, predicate: () => any) => {
    const start = performance.now();
    const result = predicate();
    const end = performance.now();
    const seconds = (end - start) / 1000;

    console.log(`${name}: ${result}`);
    console.log(`Solved in ${seconds.toFixed(3)}s.`);
};

const args = process.argv.slice(2);
if (args.length !== 2 || isNaN(Number(args[0])) || !fs.existsSync(args[1])) {
    console.log("Usage: node index.ts <day:int> <inputFilePath:string>");
    process.exit(1);
}

const [day, inputFilePath] = args;
const input = fs.readFileSync(inputFilePath, "utf8");

const solver = getSolver(+day);
if (!solver) {
    console.log("Not there yet ;)");
    process.exit(0);
}

solveWithExecutionTime("Part 1", () => solver.solvePart1(input));
solveWithExecutionTime("Part 2", () => solver.solvePart2(input));
