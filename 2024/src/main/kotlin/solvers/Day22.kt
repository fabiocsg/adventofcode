package org.example.solvers

import org.example.MrWolf

class Day22 : MrWolf {

    override fun solvePart1(input: String): Long =
        parse(input)
            .map(::getSecrets)
            .sumOf { it.last() }

    override fun solvePart2(input: String): Int {
        val incomeBySequence = mutableMapOf<Seq, Int>()

        parse(input)
            .forEach { buyerStartingPrice ->
                val prices = getSecrets(buyerStartingPrice).map { (it % 10).toInt() }
                val priceDiffs = prices.windowed(2, 1) { it[1] - it[0] }

                val sequencePrice = mutableMapOf<Seq, Int>()

                priceDiffs.windowed(4, 1)
                    .forEachIndexed { i, diffs ->
                        val seq = Seq(diffs[0], diffs[1], diffs[2], diffs[3])
                        sequencePrice.putIfAbsent(seq, prices[i + 4])
                    }

                sequencePrice.entries.forEach { (seq, price) ->
                    incomeBySequence.merge(seq, price, Int::plus)
                }
            }

        return incomeBySequence.entries.maxOf { it.value }
    }

    private fun parse(input: String): List<Long> = input.lines().map { it.toLong() }

    private fun getSecrets(source: Long): List<Long> =
        generateSequence(source) { evolve(it) }
            .take(2001)
            .toList()

    private fun evolve(source: Long): Long {
        val a = source.mixPrune(source * 64)
        val b = a.mixPrune(a / 32)
        val c = b.mixPrune(b * 2048)
        return c
    }

    private fun Long.mixPrune(other: Long) = this.xor(other) % 16777216

    private data class Seq(val a: Int, val b: Int, val c: Int, val d: Int)
}
