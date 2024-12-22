package org.example.solvers

import org.example.MrWolf

class Day12 : MrWolf {

    override fun solvePart1(input: String) = solve(input, ::findEdges)

    override fun solvePart2(input: String) = solve(input, ::findCorners)

    private fun solve(input: String, computeValue: (pos: Pos, map: Map<Pos, Char>) -> Int): Int {
        val map = parse(input)
        return findRegions(map)
            .sumOf { region ->
                region.size * region.sumOf { computeValue(it, map) }
            }
    }

    private fun parse(input: String) =
        input.lines().flatMapIndexed { y, line ->
            line.mapIndexed { x, char -> Pos(y, x) to char }
        }.toMap()

    private fun findRegions(map: Map<Pos, Char>): MutableList<List<Pos>> {
        val processed = mutableSetOf<Pos>()
        val regions = mutableListOf<List<Pos>>()

        for (pos in map.keys) {
            if (pos !in processed) {
                regions.add(expandFrom(pos, map, processed))
            }
        }

        return regions
    }

    private fun expandFrom(pos: Pos, map: Map<Pos, Char>, processed: MutableSet<Pos>): List<Pos> {
        val region = mutableListOf<Pos>()
        val queue = ArrayDeque<Pos>()
        queue.add(pos)

        while (queue.isNotEmpty()) {
            val current = queue.removeFirst()
            if (current in processed) {
                continue
            }

            processed.add(current)
            region.add(current)

            val adjacent = Direction.entries
                .map { current.move(it) }
                .filter { it !in processed && map[current] == map[it] }

            queue.addAll(adjacent)
        }

        return region
    }

    private fun findEdges(pos: Pos, map: Map<Pos, Char>) =
        Direction.entries.count { map[pos] != map[pos.move(it)] }

    private fun findCorners(pos: Pos, map: Map<Pos, Char>): Int =
        listOf(
            Direction.Up to Direction.Right,
            Direction.Right to Direction.Down,
            Direction.Down to Direction.Left,
            Direction.Left to Direction.Up
        ).sumOf { (d1, d2) -> countBends(pos, d1, d2, map) }

    private fun countBends(pos: Pos, d1: Direction, d2: Direction, map: Map<Pos, Char>): Int {
        val b1 = if (map[pos.move(d1)] != map[pos] &&
            map[pos.move(d2)] != map[pos]
        ) 1 else 0

        val b2 = if (map[pos.move(d1)] == map[pos] &&
            map[pos.move(d2)] == map[pos] &&
            map[pos.move(d1).move(d2)] != map[pos]
        ) 1 else 0

        return b1 + b2
    }


    private data class Pos(val y: Int, val x: Int) {
        fun move(dir: Direction): Pos = Pos(y + dir.dy, x + dir.dx)
    }

    private enum class Direction(val dy: Int, val dx: Int) {
        Up(-1, 0),
        Right(0, 1),
        Down(1, 0),
        Left(0, -1)
    }
}
