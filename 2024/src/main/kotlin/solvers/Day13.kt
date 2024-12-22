package org.example.solvers

import org.example.MrWolf

class Day13 : MrWolf {

    override fun solvePart1(input: String) = solve(input)

    override fun solvePart2(input: String) = solve(input, 10000000000000)

    private fun solve(input: String, add: Long = 0L): Long {
        val machines = parse(input, add)
        return machines.sumOf { tokensToSolve(it) }
    }

    private fun parse(input: String, add: Long): List<Machine> {
        return input.split("\n\n")
            .map { machineData ->
                val lines = machineData.lines()
                Machine(
                    lines[0].toVec(),
                    lines[1].toVec(),
                    lines[2].toVec(add)
                )
            }
    }

    private fun tokensToSolve(machine: Machine): Long {
        val (ay, ax) = machine.a
        val (by, bx) = machine.b
        val (prizey, prizex) = machine.prize

        val n = (prizey * ax) - (prizex * ay)
        val d = (by * ax) - (bx * ay)
        if (d == 0L) {
            return 0
        }

        val b = n.toDouble() / d
        val a = (prizex - (b * bx)) / ax

        if (a.rem(1.0) != 0.0 || b.rem(1.0) != 0.0) {
            return 0
        }

        return (a.toLong() * 3L) + b.toLong()
    }

    private fun String.toVec(add: Long = 0): Vec =
        getInts(this).let { Vec(it[0] + add, it[1] + add) }

    private fun getInts(s: String) = Regex("\\d+")
        .findAll(s)
        .map { it.value.toLong() }
        .toList()

    private data class Machine(val a: Vec, val b: Vec, val prize: Vec)

    private data class Vec(val y: Long, val x: Long) {
        operator fun plus(other: Vec) = Vec(y + other.y, x + other.x)
    }
}
