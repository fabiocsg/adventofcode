package org.example.solvers

import org.example.MrWolf

private typealias Operator = (Long, Long) -> Long
private typealias Operators = List<Operator>

class Day07 : MrWolf {

    override fun solvePart1(input: String) =
        solve(input, listOf(::add, ::mul))

    override fun solvePart2(input: String) =
        solve(input, listOf(::add, ::mul, ::comb))

    private fun solve(input: String, operators: Operators) =
        parse(input)
            .filter { isValidOperation(it.result, it.operands, operators) }
            .sumOf { it.result }

    private fun parse(input: String): List<Equation> =
        input.lines()
            .map { line ->
                val result = line.substringBefore(":").toLong()
                val operands = line.substringAfter(": ").split(" ").map { it.toLong() }
                Equation(result, operands)
            }

    private fun isValidOperation(expected: Long, values: List<Long>, operators: Operators): Boolean =
        combine(values, operators).any { it == expected }

    private fun combine(values: List<Long>, ops: Operators): List<Long> =
        if (values.size == 1) values
        else ops.map { combine(apply(it, values), ops) }.flatten()

    private fun apply(f: Operator, values: List<Long>): List<Long> =
        listOf(f(values[0], values[1])) + values.drop(2)

    private fun add(a: Long, b: Long) = a + b
    private fun mul(a: Long, b: Long) = a * b
    private fun comb(a: Long, b: Long) = "$a$b".toLong()

    private data class Equation(val result: Long, val operands: List<Long>)
}