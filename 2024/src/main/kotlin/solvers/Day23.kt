package org.example.solvers

import org.example.MrWolf

class Day23 : MrWolf {

    override fun solvePart1(input: String): Int {
        val graph = parse(input)
        return findNetworks(graph, 3)
            .count { nodes -> nodes.any { it.startsWith('t') } }
    }

    override fun solvePart2(input: String): String {
        val graph = parse(input)
        return findNetworks(graph)
            .maxBy { it.size }
            .joinToString(",")
    }

    private fun findNetworks(graph: Map<String, List<String>>, fixedSize: Int? = null): Set<List<String>> {
        val queue = ArrayDeque(graph.keys.map { listOf(it) to graph[it]!! })
        val networks = mutableSetOf<List<String>>()

        while (queue.isNotEmpty()) {
            val (current, options) = queue.removeFirst()
            if (current.size == fixedSize) {
                networks.add(current.sorted())
                continue
            }

            var expanded = false
            options.indices.forEach { i ->
                val nodes = current + options[i]
                if (allConnected(nodes, graph)) {
                    queue.add(nodes to options.drop(i))
                    expanded = true
                }
            }

            if (!expanded && fixedSize == null && current.size > 2) {
                networks.add(current.sorted())
            }
        }

        return networks.toSet()
    }

    private fun allConnected(nodes: List<String>, graph: Map<String, List<String>>) =
        nodes.pairs().all { graph[it.first]!!.contains(it.second) }

    private fun <T> List<T>.pairs(): List<Pair<T, T>> =
        indices.flatMap { i ->
            (0 until i).map { j ->
                this[i] to this[j]
            }
        }

    private fun parse(input: String): Map<String, List<String>> {
        val graph = mutableMapOf<String, List<String>>()
        input.lines()
            .map { it.split('-') }
            .forEach { (a, b) ->
                graph.merge(a, listOf(b), List<String>::plus)
                graph.merge(b, listOf(a), List<String>::plus)
            }
        return graph.toMap()
    }
}
