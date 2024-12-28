package org.example.solvers

import org.example.MrWolf

class Day19 : MrWolf {

    override fun solvePart1(input: String) = solve(input).count { it > 0 }

    override fun solvePart2(input: String) = solve(input).sum()

    private fun solve(input: String): List<Long> {
        val (towels, patterns) = parse(input)
        val cache = mutableMapOf<String, Long>()

        return patterns.map { countArrangements(it, towels, cache) }
    }

    private fun countArrangements(pattern: String, towels: List<String>, cache: MutableMap<String, Long>): Long {
        if (pattern.isEmpty()) {
            return 1
        }

        if (pattern in cache) {
            return cache[pattern]!!
        }

        val count = towels
            .filter { pattern.startsWith(it) }
            .sumOf { countArrangements(pattern.removePrefix(it), towels, cache) }

        cache[pattern] = count

        return count
    }

    private fun parse(input: String): Pair<List<String>, List<String>> {
        val (towelsString, patternsString) = input.split("\n\n")
        val towels = towelsString.split(", ")
        val patterns = patternsString.lines()
        return towels to patterns
    }
}
