package org.example.solvers

import org.example.MrWolf
import org.example.ShouldNeverHappenException

class Day15 : MrWolf {

    override fun solvePart1(input: String) = solve(input)

    override fun solvePart2(input: String) = solve(wider(input))

    private fun solve(input: String): Any {
        val (map, moves) = parse(input)
        var robot = map.entries.first { it.value == '@' }.key

        for (dir in moves) {
            robot = move(robot, dir, map)
        }

        return map.filterKeys { map[it]!! in "O[" }
            .keys.sumOf { it.y * 100 + it.x }
    }

    private fun parse(input: String): Pair<MutableMap<Pos, Char>, List<Dir>> {
        val (mapString, movesString) = input.split("\n\n")
        return parseMap(mapString) to parseMoves(movesString)
    }

    private fun parseMap(s: String): MutableMap<Pos, Char> =
        s.lines().flatMapIndexed { y, line ->
            line.mapIndexed { x, char -> Pos(y, x) to char }
        }.toMap().toMutableMap()

    private fun parseMoves(s: String): List<Dir> =
        s.replace("\n", "").map { directionFrom(it) }.toList()

    private fun move(robot: Pos, dir: Dir, map: MutableMap<Pos, Char>): Pos {
        val boxesToMove = findAffectedBoxes(robot, dir, map)
        if (!isMovePossible(boxesToMove, dir, map)) {
            return robot
        }

        updatePositions(boxesToMove, dir, map)
        return robot + dir
    }

    // also includes the robot
    private fun findAffectedBoxes(robot: Pos, dir: Dir, map: MutableMap<Pos, Char>): List<Pos> {
        val positions = mutableSetOf<Pos>()
        val queue = ArrayDeque<Pos>().apply { add(robot) }

        while (queue.isNotEmpty()) {
            val current = queue.removeFirst()
            positions.add(current)
            val next = current + dir
            if (map[next]!! in "O[]") {
                queue.add(next)
                if (map[next] == '[' && !positions.contains(next + Dir.Right)) {
                    queue.add(next + Dir.Right)
                }
                if (map[next] == ']' && !positions.contains(next + Dir.Left)) {
                    queue.add(next + Dir.Left)
                }
            }
        }

        return positions.toList()
    }

    private fun isMovePossible(boxes: List<Pos>, dir: Dir, map: MutableMap<Pos, Char>): Boolean =
        boxes
            .groupBy { if (dir == Dir.Up || dir == Dir.Down) it.x else it.y }
            .values
            .map {
                when (dir) {
                    Dir.Up -> it.minBy { pos -> pos.y }
                    Dir.Down -> it.maxBy { pos -> pos.y }
                    Dir.Left -> it.minBy { pos -> pos.x }
                    Dir.Right -> it.maxBy { pos -> pos.x }
                }
            }
            .all { map[it + dir] == '.' }

    private fun updatePositions(boxes: List<Pos>, dir: Dir, map: MutableMap<Pos, Char>) {
        val positionsToUpdate = (boxes + boxes.map { it + dir }).toSet()
        val currentValues = positionsToUpdate.associateWith { map[it]!! }

        positionsToUpdate.forEach { map[it] = currentValues[it - dir] ?: '.' }
    }

    private fun directionFrom(c: Char): Dir = when (c) {
        '^' -> Dir.Up
        '>' -> Dir.Right
        'v' -> Dir.Down
        '<' -> Dir.Left
        else -> throw ShouldNeverHappenException()
    }

    private fun wider(source: String) = source
        .replace("#", "##").replace(".", "..")
        .replace("O", "[]").replace("@", "@.")

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
