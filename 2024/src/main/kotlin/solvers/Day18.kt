package org.example.solvers

import org.example.MrWolf
import java.util.PriorityQueue

class Day18 : MrWolf {

    private val gridSize = 70

    override fun solvePart1(input: String): Int {
        val positions = parse(input)
        val corrupted = positions.take(1024)
        return findPath(corrupted)!!
    }

    override fun solvePart2(input: String): String {
        val positions = parse(input)
        var (lower, upper) = 1024 to positions.lastIndex
        do {
            val q = (lower + upper) / 2
            val corrupted = positions.take(q)
            if (findPath(corrupted) == null) {
                upper = q
            } else {
                lower = q
            }
        } while (upper - lower != 1)

        val (y, x) = positions[lower]
        return "$x,$y"
    }

    private fun findPath(corrupted: List<Pos>): Int? {
        val start = Pos(0, 0)
        val end = Pos(gridSize, gridSize)
        val queue = PriorityQueue<State>(compareBy { it.steps })
        queue.add(State(start, 0))

        val cache = mutableMapOf<Pos, Int>()

        while (queue.isNotEmpty()) {
            val state = queue.poll()
            val (pos, steps) = state

            if (cache.containsKey(pos) && cache[pos]!! <= steps) {
                continue
            }

            cache[pos] = steps
            if (pos == end) {
                return steps
            }

            Dir.entries
                .map { pos + it }
                .filter { it !in corrupted }
                .filter { it.x in 0..gridSize && it.y in 0..gridSize }
                .forEach { queue.add(State(it, steps + 1)) }
        }

        return null
    }

    private fun parse(input: String): List<Pos> = input.lines()
        .map { line -> line.split(",") }
        .map { Pos(it[1].toInt(), it[0].toInt()) }

    private data class State(val pos: Pos, val steps: Int)

    private data class Pos(val y: Int, val x: Int) {
        operator fun plus(dir: Dir) = Pos(y + dir.dy, x + dir.dx)
        operator fun minus(dir: Dir) = Pos(y - dir.dy, x - dir.dx)
    }

    private enum class Dir(val dy: Int, val dx: Int) {
        Up(-1, 0),
        Right(0, 1),
        Down(1, 0),
        Left(0, -1)
    }
}