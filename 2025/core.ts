// "I solve problems." "Good, we got one." "So I heard."
export interface MrWolf
{
    solvePart1: (input: string) => any;
    solvePart2: (input: string) => any;
}

export class ShouldNeverHappenError extends Error {
    constructor() {
        super();
        Object.setPrototypeOf(this, ShouldNeverHappenError.prototype);
    }
}