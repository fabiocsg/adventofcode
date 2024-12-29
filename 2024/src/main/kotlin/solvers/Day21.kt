package org.example.solvers

import org.example.MrWolf
import kotlin.math.abs
import kotlin.math.min

class Day21 : MrWolf {

    private val numberKeypad = parseKeypad(listOf("789", "456", "123", " 0A"))
    private val arrowKeypad = parseKeypad(listOf(" ^A", "<v>"))

    override fun solvePart1(input: String) = solve(input, 2)

    override fun solvePart2(input: String) = solve(input, 25)

    private fun solve(input: String, robots: Int): Long {
        val codes = input.split("\n")
        val keypads = listOf(numberKeypad) + List(robots) { arrowKeypad }
        val cache = mutableMapOf<State, Long>()

        return codes.sumOf { codeNumber(it) * keystrokesLength(it, keypads, cache) }
    }

    private fun codeNumber(code: String) = code.dropLast(1).toLong()

    private fun keystrokesLength(code: String, pads: List<Map<Char, Pos>>, cache: MutableMap<State, Long>) =
        if (pads.isEmpty()) code.length.toLong()
        else "A$code".windowed(2, 1)
            .sumOf { codeForKey(it[0], it[1], pads, cache) }

    private fun codeForKey(from: Char, to: Char, keypads: List<Map<Char, Pos>>, cache: MutableMap<State, Long>): Long =
        cache.getOrPut(State(from, to, keypads.size)) {
            val keypad = keypads[0]
            val posFrom = keypad[from]!!
            val posTo = keypad[to]!!

            val dy = posTo.y - posFrom.y
            val dx = posTo.x - posFrom.x
            val moveY = (if (dy < 0) "^" else "v").repeat(abs(dy))
            val moveX = (if (dx < 0) "<" else ">").repeat(abs(dx))

            val faulted = keypad[' ']!!

            // I'm still not sure why, but moving first on the x-axis or y-axis actually makes a difference
            // testing both the paths and taking the shortest one
            val xFirstSequenceSize = if (Pos(posFrom.y, posTo.x) != faulted)
                keystrokesLength(moveX + moveY + "A", keypads.drop(1), cache)
            else Long.MAX_VALUE

            val yFirstSequenceSize = if (Pos(posTo.y, posFrom.x) != faulted)
                keystrokesLength(moveY + moveX + "A", keypads.drop(1), cache)
            else Long.MAX_VALUE

            min(xFirstSequenceSize, yFirstSequenceSize)
        }

    private fun parseKeypad(rows: List<String>): Map<Char, Pos> =
        rows.flatMapIndexed { y, row ->
            row.mapIndexed { x, char -> char to Pos(y, x) }
        }.toMap()

    private data class State(val from: Char, val to: Char, val level: Int)

    private data class Pos(val y: Int, val x: Int)
}
