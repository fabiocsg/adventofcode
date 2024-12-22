package org.example.solvers

import org.example.MrWolf

class Day11 : MrWolf {

    private val cache: MutableMap<Pair<Long, Int>, Long> = mutableMapOf()

    override fun solvePart1(input: String) = solve(input, 25)

    override fun solvePart2(input: String) = solve(input, 75)

    private fun solve(input: String, blinks: Int) =
        parse(input).sumOf { transform(it, blinks) }

    private fun parse(input: String) =
        input.split(" ").map { it.toLong() }

    private fun transform(stone: Long, times: Int): Long {
        if (times == 0) {
            return 1
        }

        val key = stone to times
        if (cache.containsKey(key)){
            return cache[key]!!
        }

        val remaining = times - 1
        val res = when {
            stone == 0L -> transform(1L, remaining)
            hasEvenLength(stone) -> split(stone).sumOf { transform(it, remaining) }
            else -> transform(stone * 2024, remaining)
        }

        cache[key] = res
        return res
    }

    private fun hasEvenLength(n: Long): Boolean =
        n.toString().length % 2 == 0

    private fun split(n: Long): List<Long> {
        val s = n.toString()
        return listOf(
            s.drop(s.length / 2).toLong(),
            s.dropLast(s.length / 2).toLong()
        )
    }
}
