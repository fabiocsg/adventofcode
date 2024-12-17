package org.example.solvers

import org.example.MrWolf
import kotlin.math.abs

class Day02 : MrWolf {
    override fun solvePart1(input: String) =
        parse(input)
            .map { isSafe(it) }
            .count { it }

    override fun solvePart2(input: String) =
        parse(input)
            .map { isSafeWithDampener(it) }
            .count { it }

    private fun parse(input: String): List<List<Int>> {
        return input.lines()
            .map { l -> l.split(" ").map(String::toInt) }
    }

    private fun isSafe(report: List<Int>): Boolean {
        val steps = report
            .zipWithNext { a, b -> a - b }

        return steps.distinctBy { it > 0 }.size == 1
                && steps.all { abs(it) in 1..3 }
    }

    private fun isSafeWithDampener(report: List<Int>) =
        isSafe(report) || report.indices
            .map { report.filterIndexed { index, _ -> index != it } }
            .any { isSafe(it) }
}

