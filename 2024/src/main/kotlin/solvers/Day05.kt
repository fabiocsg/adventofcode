package org.example.solvers

import org.example.MrWolf

class Day05 : MrWolf {

    override fun solvePart1(input: String): Any {
        val (updates, comparator) = parse(input)
        return updates
            .filter { isSorted(it, comparator) }
            .sumOf { it[(it.size / 2)] }
    }

    override fun solvePart2(input: String): Any {
        val (updates, comparator) = parse(input)
        return updates
            .filter { !isSorted(it, comparator) }
            .map { it.sortedWith(comparator) }
            .sumOf { it[(it.size / 2)] }
    }

    private fun parse(input: String): Pair<List<List<Int>>, Comparator<Int>> {
        val (rulesString, updatesString) = input.split("\n\n")

        val updates = updatesString.lines()
            .map { row ->
                row.split(",").map { it.toInt() }
            }

        val comparator = comparatorFrom(rulesString.lines())

        return updates to comparator
    }

    private fun comparatorFrom(rules: List<String>): Comparator<Int> =
        Comparator { a, b -> if (rules.contains("$a|$b")) -1 else 1 }

    private fun <T> isSorted(list: List<T>, comparator: Comparator<T>) =
        list == list.sortedWith(comparator)
}

