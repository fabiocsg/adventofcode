package org.example.solvers

import org.example.MrWolf
import org.example.ShouldNeverHappenException
import java.util.PriorityQueue
import kotlin.math.abs

class Day20 : MrWolf {

    override fun solvePart1(input: String) = solve(input, 2)

    override fun solvePart2(input: String) = solve(input, 20)

    private fun solve(input: String, maxCheatTime: Int): Int {
        val (maze, start, end) = parse(input)
        val path = findPath(maze, start, end)
        return countCheats(path, maxCheatTime)
    }

    private fun findPath(maze: Map<Pos, Char>, start: Pos, end: Pos): List<Pos> {
        val queue = PriorityQueue<List<Pos>>(compareBy { it.size })
        queue.add(listOf(start))

        while (queue.isNotEmpty()) {
            val path = queue.poll()
            val lastPos = path.last()
            if (lastPos == end) {
                return path
            }

            Dir.entries.map { lastPos + it }
                .filter { maze[it]!! != '#' && it !in path }
                .forEach { queue.add(path + it) }
        }

        throw ShouldNeverHappenException()
    }

    private fun countCheats(path: List<Pos>, maxCheatTime: Int): Int =
        path.indices.sumOf { i ->
            (0 until i).count { j ->
                val distance = calculateDistance(path[j], path[i])
                val cut = i - (j + distance)
                cut >= 100 && distance <= maxCheatTime
            }
        }

    private fun calculateDistance(a: Pos, b: Pos) = abs(a.y - b.y) + abs(a.x - b.x)

    private fun parse(input: String): Triple<Map<Pos, Char>, Pos, Pos> {
        val maze = input.lines().flatMapIndexed { y, line ->
            line.mapIndexed { x, char -> Pos(y, x) to char }
        }.toMap()

        val start = maze.entries.find { it.value == 'S' }!!.key
        val end = maze.entries.find { it.value == 'E' }!!.key
        return Triple(maze, start, end)
    }

    private data class Pos(val y: Int, val x: Int) {
        operator fun plus(dir: Dir) = Pos(y + dir.dy, x + dir.dx)
    }

    private enum class Dir(val dy: Int, val dx: Int) {
        Up(-1, 0),
        Right(0, 1),
        Down(1, 0),
        Left(0, -1)
    }
}
