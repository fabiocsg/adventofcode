package org.example

import java.io.File
import java.io.IO.println
import kotlin.reflect.full.createInstance
import kotlin.system.measureTimeMillis

fun main(args: Array<String>) {
    if (args.size != 2 || args[0].toIntOrNull() == null || !File(args[1]).exists()) {
        println("Invalid Arguments. Usage: {day:int} {inputFilePath:string}")
        return
    }

    val day = args[0].toInt()
    val inputFilePath = args[1]
    val input = File(inputFilePath).readText()

    val solver = getSolverForDay(day)

    if (solver == null) {
        println("Not there yet ;)")
        return
    }

    solveWithExecutionTime("Part 1") { solver.solvePart1(input) }
    solveWithExecutionTime("Part 2") { solver.solvePart2(input) }
}

private fun getSolverForDay(day: Int): MrWolf? {
    val days = day.toString().padStart(2, '0')
    val className = "org.example.solvers.Day$days"

    return try {
        Class.forName(className).kotlin.createInstance() as MrWolf
    } catch (e: Exception) {
        null
    }
}

private fun solveWithExecutionTime(name: String, predicate: () -> Any) {
    val time = measureTimeMillis {
        val result = predicate()
        println("$name: $result")
    }
    println("Solved in ${time / 1000.0}s\n")
}

