package org.example.solvers

import org.example.MrWolf

class Day06 : MrWolf {

    override fun solvePart1(input: String): Any {
        val (map, start) = parse(input)
        return navigate(map, start).first.size
    }

    override fun solvePart2(input: String): Any {
        val (map, start) = parse(input)
        val (steps, _) = navigate(map, start)
        return steps.count { map[it] == '.' && canLoopWithBlock(it, map, start) }
    }

    private fun parse(input: String): Pair<MutableMap<Pos, Char>, Pos> {
        val map = mutableMapOf<Pos, Char>()
        input.lines().forEachIndexed { y, row ->
            row.forEachIndexed { x, char ->
                map[Pos(y, x)] = char
            }
        }

        return map to map.entries.first { it.value == '^' }.key
    }

    private fun navigate(map: MutableMap<Pos, Char>, start: Pos): Pair<Set<Pos>, Boolean> {
        val steps = mutableSetOf<Step>()
        var pos = start
        var dir = Dir.Up

        while (isInBounds(map, pos) && !steps.contains(Step(pos, dir))) {
            steps.add(Step(pos, dir))

            val nextPos = pos.move(dir)
            if (map.getOrDefault(nextPos, null) == '#') {
                dir = dir.next()
            } else {
                pos = nextPos
            }
        }

        return steps.map { it.pos }.toSet() to steps.contains(Step(pos, dir))
    }

    private fun canLoopWithBlock(pos: Pos, map: MutableMap<Pos, Char>, start: Pos): Boolean {
        map[pos] = '#'
        val canLoop = navigate(map, start).second
        map[pos] = '.'
        return canLoop
    }

    private fun isInBounds(map: MutableMap<Pos, Char>, pos: Pos) = map.containsKey(pos)

    private data class Step(val pos: Pos, val dir: Dir)

    private data class Pos(val y: Int, val x: Int) {
        fun move(direction: Dir): Pos = Pos(y + direction.dy, x + direction.dx)
    }

    private enum class Dir(val dy: Int, val dx: Int) {
        Up(-1, 0),
        Right(0, 1),
        Down(1, 0),
        Left(0, -1);

        fun next(): Dir = when (this) {
            Up -> Right
            Right -> Down
            Down -> Left
            Left -> Up
        }
    }

}
