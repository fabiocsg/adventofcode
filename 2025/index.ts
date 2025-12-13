import * as fs from "node:fs";

import type {MrWolf} from "./core.ts";
import {day01} from "./solvers/day01.ts";
import {day02} from "./solvers/day02.ts";
import {day03} from "./solvers/day03.ts";
import {day04} from "./solvers/day04.ts";
import {day05} from "./solvers/day05.ts";
import {day06} from "./solvers/day06.ts";
import {day07} from "./solvers/day07.ts";
import {day08} from "./solvers/day08.ts";
import {day09} from "./solvers/day09.ts";
import {day10} from "./solvers/day10.ts";
import {day11} from "./solvers/day11.ts";
import {day12} from "./solvers/day12.ts";


const solvers: Record<number, MrWolf> = {
    1: day01,
    2: day02,
    3: day03,
    4: day04,
    5: day05,
    6: day06,
    7: day07,
    8: day08,
    9: day09,
    10: day10,
    11: day11,
    12: day12,
};

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

const solver = solvers[+day];
if (!solver) {
    console.log("Not there yet ;)");
    process.exit(0);
}

solveWithExecutionTime("Part 1", () => solver.solvePart1(input));
solveWithExecutionTime("Part 2", () => solver.solvePart2(input));
