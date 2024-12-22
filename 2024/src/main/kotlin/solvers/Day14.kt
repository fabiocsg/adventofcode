package org.example.solvers

import org.example.MrWolf

class Day14 : MrWolf {

    private val sizeX = 101
    private val sizeY = 103

    override fun solvePart1(input: String): Int {
        val guards = parse(input)
        return guards
            .map { positionAfter(it, 100) }
            .mapNotNull { findQuadrant(it) }
            .groupingBy { it }
            .eachCount()
            .values
            .reduce { acc, entry -> acc * entry }
    }

    override fun solvePart2(input: String): Int {
        val guards = parse(input)

        for (time in 1..10000) {
            val positions = guards
                .map { positionAfter(it, time) }
                .toSet()

            if (positions.any { hasManyNeighbors(it, positions) }) {
                return time
            }
        }

        return 0
    }

    private fun parse(input: String): List<Robot> {
        val regex = Regex("p=(-?\\d+),(-?\\d+) v=(-?\\d+),(-?\\d+)")
        return input.lines()
            .mapNotNull {
                regex.find(it)?.destructured?.let { (x, y, dx, dy) ->
                    Robot(Vec(y.toInt(), x.toInt()), Vec(dy.toInt(), dx.toInt()))
                }
            }
    }

    private fun positionAfter(robot: Robot, seconds: Int): Vec {
        val posY = robot.pos.y + (robot.vel.y * seconds) % sizeY
        val posX = robot.pos.x + (robot.vel.x * seconds) % sizeX

        return Vec((posY + sizeY) % sizeY, (posX + sizeX) % sizeX)
    }

    private fun findQuadrant(pos: Vec): Int? {
        val halfX = (sizeX / 2)
        val halfY = (sizeY / 2)

        return when {
            pos.y < halfY && pos.x < halfX -> 1
            pos.y < halfY && pos.x > halfX -> 2
            pos.y > halfY && pos.x < halfX -> 3
            pos.y > halfY && pos.x > halfX -> 4
            else -> null
        }
    }

    private fun hasManyNeighbors(pos: Vec, positions: Set<Vec>): Boolean {
        val many = 20 // random threshold
        repeat(many) { i ->
            if (!positions.contains(Vec(pos.y, pos.x + i))) {
                return false
            }
        }

        return true
    }

    data class Vec(val y: Int, val x: Int)
    data class Robot(val pos: Vec, val vel: Vec)
}
