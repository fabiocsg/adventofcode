package org.example.solvers

import org.example.MrWolf

class Day03 : MrWolf {
    override fun solvePart1(input: String) =
        parseMul(input)
            .sumOf { (a, b) -> a * b }

    override fun solvePart2(input: String) =
        parseDos(input)
            .flatMap(::parseMul)
            .sumOf { (a, b) -> a * b }

    private fun parseMul(input: String) =
        Regex("mul\\((\\d{1,3}),\\s*(\\d{1,3})\\)")
            .findAll(input)
            .map { it.groupValues[1].toInt() to it.groupValues[2].toInt() }
            .toList()

    private fun parseDos(input: String) =
        input.split("do()")
            .map { it.split("don't()")[0] }
}
