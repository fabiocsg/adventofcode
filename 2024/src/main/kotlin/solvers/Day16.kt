package org.example.solvers

import org.example.MrWolf
import java.util.PriorityQueue

class Day16 : MrWolf {

    override fun solvePart1(input: String): Int {
        val (maze, start, end) = parse(input)

        return navigate(maze, start, end).entries
            .filter { it.key.first == end }
            .minBy { it.value }
            .value
    }

    override fun solvePart2(input: String): Int {
        val (maze, start, end) = parse(input)
        val visited = navigate(maze, start, end)

        return countBestSeats(maze, start, end, visited)
    }

    private fun parse(input: String): Triple<Map<Pos, Char>, Pos, Pos> {
        val maze = input.lines().flatMapIndexed { y, line ->
            line.mapIndexed { x, char -> Pos(y, x) to char }
        }.toMap()

        val start = maze.entries.find { it.value == 'S' }!!.key
        val end = maze.entries.find { it.value == 'E' }!!.key
        return Triple(maze, start, end)
    }

    private fun navigate(maze: Map<Pos, Char>, start: Pos, end: Pos): Map<Pair<Pos, Dir>, Int> {
        val queue = PriorityQueue<State>(compareBy { it.score })
        queue.add(State(start, Dir.East, 0))

        val cache = mutableMapOf<Pair<Pos, Dir>, Int>()

        while (queue.isNotEmpty()) {
            val state = queue.poll()
            val (pos, dir, score) = state

            if (cache.containsKey(pos to dir) && cache[pos to dir]!! <= score) {
                continue
            }

            cache[pos to dir] = score
            if (pos == end) {
                continue
            }

            nextStates(state, maze).forEach { queue.add(it) }
        }

        return cache.toMap()
    }

    private fun countBestSeats(maze: Map<Pos, Char>, start: Pos, end: Pos, visited: Map<Pair<Pos, Dir>, Int>): Int {
        val seats = mutableSetOf(end)
        val queue = ArrayDeque<State>()

        val scores = visited.entries
            .groupBy { it.key.first }
            .map { it.key to it.value.minBy { e -> e.value }.value }
            .toMap()

        val bestEndScore = scores[end]!!

        Dir.entries.forEach { queue.add(State(end, it, bestEndScore)) }

        while (queue.isNotEmpty()) {
            val state = queue.removeFirst()
            if (state.pos == start) {
                continue
            }

            nextStates(state, maze, true)
                .filter { nextState ->
                    scores.containsKey(nextState.pos) &&
                            scores[nextState.pos]!! <= nextState.score
                }
                .forEach { nextState ->
                    queue.add(nextState)
                    seats.add(nextState.pos)
                }
        }

        return seats.size
    }

    private fun nextStates(state: State, maze: Map<Pos, Char>, backwards: Boolean = false): List<State> {
        val (pos, dir, score) = state

        return Dir.entries.mapNotNull { nextDir ->
            val nextPos = pos + nextDir
            if (nextPos == pos - dir || maze[nextPos] == '#') {
                null
            } else {
                val stepValue = if (nextDir == dir) 1 else 1001
                val nextScore = if (backwards) score - stepValue else score + stepValue
                State(nextPos, nextDir, nextScore)
            }
        }
    }

    private data class State(val pos: Pos, val dir: Dir, val score: Int)

    private data class Pos(val y: Int, val x: Int) {
        operator fun plus(dir: Dir) = Pos(y + dir.dy, x + dir.dx)
        operator fun minus(dir: Dir) = Pos(y - dir.dy, x - dir.dx)
    }

    private enum class Dir(val dy: Int, val dx: Int) {
        North(-1, 0),
        East(0, 1),
        South(1, 0),
        West(0, -1)
    }
}
