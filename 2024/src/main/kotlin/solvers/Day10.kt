package org.example.solvers

import org.example.MrWolf

class Day10 : MrWolf {

    override fun solvePart1(input: String) = solve(input, false)

    override fun solvePart2(input: String) = solve(input, true)

    private fun solve(input: String, countDifferentPaths: Boolean): Int {
        val map = parse(input)
        return map.entries
            .asSequence()
            .filter { it.value == 0 }
            .map { it.key }
            .map { findPeaksFrom(it, map) }
            .map { if (countDifferentPaths) it else it.distinct() }
            .sumOf { it.size }
    }

    private fun parse(input: String) =
        input.lines().flatMapIndexed { y, line ->
            line.mapIndexed { x, char -> Pos(y, x) to char.digitToInt() }
        }.toMap()

    private fun findPeaksFrom(pos: Pos, map: Map<Pos, Int>): List<Pos> {
        if (map[pos] == 9) {
            return listOf(pos)
        }

        return Dir.entries
            .map { pos.move(it) }
            .filter { map.containsKey(it) && map[it] == 1 + map[pos]!! }
            .flatMap { findPeaksFrom(it, map) }
    }

    private data class Pos(val y: Int, val x: Int) {
        fun move(direction: Dir): Pos = Pos(y + direction.dy, x + direction.dx)
    }

    private enum class Dir(val dy: Int, val dx: Int) {
        Up(-1, 0),
        Right(0, 1),
        Down(1, 0),
        Left(0, -1)
    }
}