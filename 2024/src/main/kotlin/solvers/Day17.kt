package org.example.solvers

import org.example.MrWolf
import org.example.ShouldNeverHappenException

class Day17 : MrWolf {

    override fun solvePart1(input: String): Any {
        val (register, instructions) = parse(input)

        return runProgram(register, instructions)
            .joinToString(",") { it.toString() }
    }

    // couldn't figure out part2 on my own, thanks reddit for the hint
    override fun solvePart2(input: String): Any {
        val instructions = parse(input).second
        val sequence = instructions.flatMap { listOf(it.opcode, it.operand) }

        var stepValues = listOf(0L)

        for (step in sequence.indices) {
            val expected = sequence.takeLast(step + 1)
            stepValues = stepValues.flatMap { value ->
                (0 until 8).mapNotNull { i ->
                    val a = value * 8 + i
                    val output = runProgram(Register(a, 0L, 0L), instructions)
                    if (output == expected) a else null
                }
            }
        }

        return stepValues.min()
    }

    private fun parse(input: String): Pair<Register, List<Instruction>> {
        val (regString, programString) = input.split("\n\n")
        val values = regString.lines().map { it.split(": ")[1].toLong() }
        val register = Register(values[0], values[1], values[2])
        val instructions = programString
            .substringAfter(": ")
            .split(',')
            .map(String::toLong)
            .chunked(2)
            .map { Instruction(it[0], it[1]) }

        return register to instructions
    }

    private fun runProgram(register: Register, instructions: List<Instruction>): List<Long> {
        var pointer = 0
        val output = mutableListOf<Long>()

        while (pointer <= instructions.lastIndex) {
            val (opcode, operand) = instructions[pointer]
            val combo = combo(operand, register)
            pointer += 1

            when (opcode) {
                0L -> register.a /= (1 shl combo.toInt())
                1L -> register.b = register.b.xor(operand)
                2L -> register.b = combo % 8
                3L -> pointer = if (register.a == 0L) pointer else (operand / 2).toInt()
                4L -> register.b = register.b.xor(register.c)
                5L -> output.add(combo % 8)
                6L -> register.b = register.a / (1 shl combo.toInt())
                7L -> register.c = register.a / (1 shl combo.toInt())
            }
        }

        return output.toList()
    }

    private fun combo(operand: Long, register: Register): Long = when (operand) {
        0L, 1L, 2L, 3L -> operand
        4L -> register.a
        5L -> register.b
        6L -> register.c
        else -> throw ShouldNeverHappenException()
    }

    private data class Register(var a: Long, var b: Long, var c: Long)
    private data class Instruction(val opcode: Long, val operand: Long)
}
