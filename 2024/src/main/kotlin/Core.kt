package org.example

// "I solve problems." "Good, we got one." "So I heard."
interface MrWolf {
    fun solvePart1(input: String): Any
    fun solvePart2(input: String): Any
}

class ShouldNeverHappenException : Exception()