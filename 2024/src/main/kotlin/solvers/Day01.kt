package org.example.solvers

import org.example.MrWolf
import kotlin.math.abs

class Day01 : MrWolf {
    override fun solvePart1(input: String): Any {
        val (l1, l2) = parse(input)
        return l1.sorted()
            .zip(l2.sorted())
            .sumOf { (a, b) -> abs(a - b) }
    }

    override fun solvePart2(input: String): Any {
        val (l1, l2) = parse(input)
        return l1.sumOf { v -> v * l2.count { it == v } }
    }

    private fun parse(input: String): Pair<List<Int>, List<Int>> {
        val pairs = input.lines()
            .map { l -> l.split("   ").map(String::toInt) }

        val l1 = pairs.map { it[0] }
        val l2 = pairs.map { it[1] }
        return Pair(l1, l2)
    }
}