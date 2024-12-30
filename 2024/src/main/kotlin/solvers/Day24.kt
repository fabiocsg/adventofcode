package org.example.solvers

import org.example.MrWolf

class Day24 : MrWolf {

    override fun solvePart1(input: String): Long {
        val (wires, gates) = parse(input)
        return gates
            .filter { it.name.startsWith("z") }
            .sortedByDescending { it.name }
            .map { findNodeValue(it.name, gates, wires) }
            .joinToString("") { if (it) "1" else "0" }
            .toLong(2)
    }

    override fun solvePart2(input: String): String {
        val (_, gates) = parse(input)
        val lastOutput = gates.map { it.name }
            .filter { it.startsWith("z") }
            .max()

        return gates.filter { isMisplaced(it, gates, lastOutput) }
            .map { it.name }
            .sorted()
            .joinToString(",")
    }

    private fun findNodeValue(name: String, gates: List<Gate>, wires: Map<String, Boolean>): Boolean {
        if (wires.containsKey(name)) {
            return wires[name]!!
        }
        val gate = gates.find { it.name == name }!!
        val v1 = findNodeValue(gate.i1, gates, wires)
        val v2 = findNodeValue(gate.i2, gates, wires)
        return when (gate.op) {
            Op.AND -> v1 && v2
            Op.OR -> v1 || v2
            Op.XOR -> v1 != v2
        }
    }

    private fun isMisplaced(gate: Gate, gates: List<Gate>, lastOutput: String): Boolean =
        (outputWithoutXor(gate) && gate.name != lastOutput)
                || internalWithXor(gate)
                || (inputWithUnexpectedChildren(gate, Op.XOR, Op.XOR, gates) && !isFirstInput(gate))
                || (inputWithUnexpectedChildren(gate, Op.AND, Op.OR, gates) && !isFirstInput(gate))

    private fun outputWithoutXor(gate: Gate) = isOutput(gate) && gate.op != Op.XOR
    private fun internalWithXor(gate: Gate) = !isOutput(gate) && !isInput(gate) && gate.op == Op.XOR

    private fun inputWithUnexpectedChildren(gate: Gate, op: Op, expectedChildOp: Op, gates: List<Gate>): Boolean =
        isInput(gate) && !isFirstInput(gate) && gate.op == op &&
                gates.filter { it.i1 == gate.name || it.i2 == gate.name }
                    .all { it.op != expectedChildOp }

    private fun isOutput(gate: Gate) = gate.name.startsWith("z")

    private fun isInput(gate: Gate) = listOf(gate.i1, gate.i2).any {
        it.startsWith("y") || it.startsWith("x")
    }

    private fun isFirstInput(gate: Gate) = isInput(gate)
            && gate.i1.endsWith("00")
            && gate.i2.endsWith("00")

    private fun parse(input: String): Pair<Map<String, Boolean>, List<Gate>> {
        val (wiresString, gatesString) = input.split("\n\n")
        val wires = wiresString.lines()
            .map { it.split(": ") }
            .associate { (name, value) -> name to (value == "1") }

        val regex = Regex("([a-z0-9]+) ([A-Z]+) ([a-z0-9]+) -> ([a-z0-9]+)")
        val gates = gatesString.lines()
            .mapNotNull {
                regex.find(it)?.destructured?.let { (i1, op, i2, name) ->
                    Gate(name, Op.valueOf(op), i1, i2)
                }
            }

        return wires to gates
    }

    private data class Gate(val name: String, val op: Op, val i1: String, val i2: String)
    private enum class Op { AND, OR, XOR }
}
