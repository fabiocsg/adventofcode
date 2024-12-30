package org.example.solvers

import org.example.MrWolf

class Day25 : MrWolf {

    override fun solvePart1(input: String): Any {
        val (keys, locks) = parse(input)
        return keys.sumOf { key ->
            locks.count { lock ->
                (0..4).all { key[it] + lock[it] < 6 }
            }
        }
    }

    override fun solvePart2(input: String) = "🥳🥳🥳"

    private fun parse(input: String): Pair<List<List<Int>>, List<List<Int>>> {
        val keys = mutableListOf<List<Int>>()
        val locks = mutableListOf<List<Int>>()
        input.split("\n\n").forEach { schema ->
            val rows = schema.lines()
            val isKey = schema.startsWith('.')
            val counts = (0..4).map {
                rows.count { row -> row[it] == '#' } - 1
            }

            if (isKey) keys.add(counts) else locks.add(counts)
        }

        return keys.toList() to locks.toList()
    }
}
