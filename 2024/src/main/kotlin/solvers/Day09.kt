package org.example.solvers

import org.example.MrWolf

class Day09 : MrWolf {

    override fun solvePart1(input: String): Any {
        val blocks = parse(input)
        var start = 0
        var end = blocks.lastIndex

        while (start != end) {
            when {
                blocks[start].isFull() -> start++
                blocks[end].isEmpty() -> end--
                else -> blocks[start].insert(blocks[end].pop())
            }
        }

        return checksum(blocks)
    }

    override fun solvePart2(input: String): Any {
        val blocks = parse(input).toMutableList()
        var sourceIndex = blocks.lastIndex
        while (sourceIndex >= 0) {
            val source = blocks[sourceIndex]
            if (source.isEmpty()) {
                sourceIndex--
                continue
            }

            val targetIndex = (0 until sourceIndex)
                .firstOrNull { blocks[it].freeSpaceSize() >= source.dataSize() }

            if (targetIndex == null) {
                sourceIndex--
                continue
            }

            val target = blocks[targetIndex]
            val newBlock = target.splitFreeSpace()
            blocks.add(targetIndex + 1, newBlock)
            repeat(source.dataSize()) { newBlock.insert(source.pop()) }
        }

        return checksum(blocks)
    }

    private fun parse(input: String) =
        input
            .chunked(2)
            .mapIndexed { index, value ->
                val dataSize = value[0].digitToInt()
                val freeSize = value.getOrNull(1)?.digitToInt() ?: 0
                val data = List(dataSize) { index } + List(freeSize) { -1 }
                Block(data.toMutableList())
            }

    private fun checksum(blocks: List<Block>): Long =
        blocks
            .flatMap { it.data }
            .withIndex()
            .sumOf { if (it.value == -1) 0L else it.value * it.index.toLong() }


    private data class Block(val data: MutableList<Int>) {
        fun hasFreeSpace() = -1 in data
        fun freeSpaceSize() = data.count { it == -1 }
        fun dataSize() = data.count { it != -1 }
        fun isEmpty() = data.all { it == -1 }
        fun isFull() = !hasFreeSpace()
        fun pop(): Int {
            val index = data.indexOfLast { it != -1 }
            val value = data[index]
            data[index] = -1
            return value
        }

        fun insert(value: Int) {
            val index = data.indexOfFirst { it == -1 }
            data[index] = value
        }

        fun splitFreeSpace(): Block {
            val freeSize = freeSpaceSize()
            data.removeAll { it == -1 }
            return Block(List(freeSize) { -1 }.toMutableList())
        }
    }
}