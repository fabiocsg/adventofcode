package org.example.solvers

import org.example.MrWolf

private data class Pos(val y: Int, val x: Int)
private typealias AntinodesResolver = (Pos, Pos, Map<Pos, Char>) -> List<Pos>

class Day08 : MrWolf {

    override fun solvePart1(input: String) = solve(input, ::findAntinodes)

    override fun solvePart2(input: String) = solve(input, ::findAntinodesLine)

    private fun solve(input: String, solver: AntinodesResolver): Int {
        val map = parse(input)
        return map.entries
            .filter { it.value != '.' }
            .groupBy({ it.value }, { it.key })
            .values
            .flatMap(::getPairs)
            .flatMap { solver(it.first, it.second, map) }
            .toSet()
            .count()
    }

    private fun parse(input: String): Map<Pos, Char> =
        input.lines().flatMapIndexed { y, line ->
            line.mapIndexed { x, char -> Pos(y, x) to char }
        }.toMap()

    private fun getPairs(positions: List<Pos>) =
        positions.flatMapIndexed { index, p1 ->
            positions.drop(index + 1).map { p2 -> p1 to p2 }
        }

    private fun findAntinodes(p1: Pos, p2: Pos, map: Map<Pos, Char>): List<Pos> {
        val dy = p2.y - p1.y
        val dx = p2.x - p1.x
        return listOf(
            Pos(p1.y - dy, p1.x - dx),
            Pos(p2.y + dy, p2.x + dx)
        ).filter { map.containsKey(it) }
    }

    private fun findAntinodesLine(p1: Pos, p2: Pos, map: Map<Pos, Char>): List<Pos> {
        val dy = p2.y - p1.y
        val dx = p2.x - p1.x

        val a = generateSequence(p1) { Pos(it.y - dy, it.x - dx) }
            .takeWhile { map.containsKey(it) }

        val b = generateSequence(p2) { Pos(it.y + dy, it.x + dx) }
            .takeWhile { map.containsKey(it) }

        return (a + b).toList()
    }
}