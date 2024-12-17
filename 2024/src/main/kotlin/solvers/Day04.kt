package org.example.solvers

import org.example.MrWolf

class Day04 : MrWolf {

    override fun solvePart1(input: String): Any {
        val grid = parse(input)
        return grid.indices.sumOf { y ->
            grid[y].indices.sumOf { x ->
                directions.count { dir -> findWord(grid, x, y, dir, "XMAS") }
            }
        }
    }

    override fun solvePart2(input: String): Any {
        val grid = parse(input)
        return grid.indices.sumOf { y ->
            grid[y].indices.count { x -> findCross(grid, x, y) }
        }
    }

    private fun parse(input: String) = input.lines().map { it.toList() }

    private fun findWord(grid: List<List<Char>>, x: Int, y: Int, dir: Direction, word: String): Boolean {
        for (index in word.indices) {
            val posX = x + dir.x * index
            val posY = y + dir.y * index
            if (!isInBounds(grid, posX, posY) || grid[posY][posX] != word[index])
                return false
        }
        return true
    }

    private fun findCross(grid: List<List<Char>>, x: Int, y: Int): Boolean {
        if (grid[y][x] != 'A') {
            return false
        }
        val upLeft = grid.getOrNull(y - 1)?.getOrNull(x - 1)
        val upRight = grid.getOrNull(y - 1)?.getOrNull(x + 1)
        val downRight = grid.getOrNull(y + 1)?.getOrNull(x + 1)
        val downLeft = grid.getOrNull(y + 1)?.getOrNull(x - 1)

        val d1 = "${upLeft}A${downRight}"
        val d2 = "${downLeft}A${upRight}"

        return (d1 == "MAS" || d1 == "SAM") && (d2 == "MAS" || d2 == "SAM")
    }

    private fun isInBounds(grid: List<List<Char>>, x: Int, y: Int): Boolean =
        y in grid.indices && x in grid[y].indices
}

private data class Direction(val x: Int, val y: Int)

private val directions = listOf(
    Direction(1, 0),    // Right
    Direction(1, 1),    // Down-Right
    Direction(0, 1),    // Down
    Direction(-1, 1),   // Down-Left
    Direction(-1, 0),   // Left
    Direction(-1, -1),  // Up-Left
    Direction(0, -1),   // Up
    Direction(1, -1),   // Up-Right
)
