using aoc2023.Core;

namespace aoc2023.Solvers;

[Day(24)]
internal sealed class Day24 : IMrWolf
{
    public object SolvePart1(string input)
    {
        var hailstones = ParseHailstones(input);

        const long boundsMin = 200_000_000_000_000;
        const long boundsMax = 400_000_000_000_000;

        return GetPairs(hailstones)
            .Count(pair => IntersectsWithinBounds(pair.Item1, pair.Item2, boundsMin, boundsMax));
    }

    public object SolvePart2(string input)
    {
        var hailstones = ParseHailstones(input);

        var c = SolveEquation(hailstones)
            .Select(x => Convert.ToInt64(Math.Round(x)))
            .ToArray();
        
        var rock = new Hailstone(new Vec(c[0], c[1], c[2]), new Vec(c[3], c[4], c[5]));
        return rock.Pos.X + rock.Pos.Y + rock.Pos.Z;
    }

    private static bool IntersectsWithinBounds(Hailstone h1, Hailstone h2, long min, long max)
    {
        var determinant = h1.Vel.X * h2.Vel.Y - h1.Vel.Y * h2.Vel.X;

        if (determinant == 0)
        {
            return false;
        }

        var dy = h1.Pos.Y - h2.Pos.Y;
        var dx = h2.Pos.X - h1.Pos.X;
        var t1 = (h2.Vel.X * dy + h2.Vel.Y * dx) / determinant;
        var t2 = (h1.Vel.X * dy + h1.Vel.Y * dx) / determinant;

        if (t1 < 0 || t2 < 0)
        {
            return false;
        }

        var x = h2.Pos.X + h2.Vel.X * t2;
        var y = h2.Pos.Y + h2.Vel.Y * t2;

        return x >= min && x <= max && y >= min && y <= max;
    }

    private static decimal[] SolveEquation(Hailstone[] hailstones)
    {
        // The rock we throw has a position RX RY RZ and a velocity RVX RVY RVZ
        // for every axis we have a position (t)
        // PosRX = RX + RVXt
        //
        // the same is true for every hailstone H
        // PosHX = HX + HVXt
        //
        // so
        // RX + RVXt = HX + HVXt
        // t = (RX-HX)/(HVX-RVX)
        //
        // same thing for the Y axis so we can equate the two
        // (RX-HX) / (HVX-RVX) = (RY-HY) / (HVY-RVY)
        // (RX-HX)(HVY-RVY) = (RY-HY)(HVX-RVX)
        // RX*HVY - RX*RVY - HX*HVY + HX*RVY = RY*HVX - RY*RVX - HY*HVX + HY*RVX
        // 
        // isolating the unknowns on the left hand side
        // RY*RVX - RX*RVY = RY*HVX - HY*HVX + HY*RVX - RX*HVY + HX*HVY - HX*RVY
        //
        // we can say the same thing for every hailstone, taking in consideration H'
        // RY*RVX - RX*RVY = RY*HVX - HY*HVX + HY*RVX - RX*HVY + HX*HVY - HX*RVY
        // RY*RVX - RX*RVY = RY*HVX' - HY'*HVX' + HY'*RVX - RX*HVY' + HX'*HVY' - HX'*RVY
        //
        // equating the two
        // RY*HVX - HY*HVX + HY*RVX - RX*HVY + HX*HVY - HX*RVY = RY*HVX' - HY'*HVX' + HY'*RVX - RX*HVY' + HX'*HVY' - HX'*RVY
        //
        // we can isolate the 4 unknowns RX RY RVX RVY
        // RX(HVY'-HVY) + RY(HVX-HVX') + RVX(HY-HY') + RVY(HX'-HX) = HY*HVX - HX*HVY - HY'*HVX' + HX'*HVY'
        //
        // the same equation holds for every pair XY XZ and YZ, which gives us 
        // RX(HVY'-HVY) + RY(HVX-HVX') + RVX(HY-HY') + RVY(HX'-HX) = HY*HVX - HX*HVY - HY'*HVX' + HX'*HVY'
        // RX(HVZ'-HVZ) + RZ(HVX-HVX') + RVX(HZ-HZ') + RVZ(HX'-HX) = HZ*HVX - HX*HVZ - HZ'*HVX' + HX'*HVZ'
        // RY(HVZ'-HVZ) + RZ(HVY-HVY') + RVY(HZ-HZ') + RVZ(HY'-HY) = HZ*HVY - HY*HVZ - HZ'*HVY' + HY'*HVZ'
        //
        // adding a third hailstone to the system gives us 6 equations with 6 unknows
        // we can solve it using gaussian elimination
        // https://en.wikipedia.org/wiki/Gaussian_elimination

        var matrix = new decimal[6][];
        // array composed as [X Y Z VX VY VZ .]
        matrix[0] = ComposeXY(hailstones[0], hailstones[1]);
        matrix[1] = ComposeXZ(hailstones[0], hailstones[1]);
        matrix[2] = ComposeYZ(hailstones[0], hailstones[1]);
        matrix[3] = ComposeXY(hailstones[0], hailstones[2]);
        matrix[4] = ComposeXZ(hailstones[0], hailstones[2]);
        matrix[5] = ComposeYZ(hailstones[0], hailstones[2]);
        return SolveMatrix(matrix);
    }

    // ReSharper disable once InconsistentNaming
    // RX(HVY'-HVY) + RY(HVX-HVX') + RVX(HY-HY') + RVY(HX'-HX) = HY*HVX - HX*HVY - HY'*HVX' + HX'*HVY'
    private static decimal[] ComposeXY(Hailstone h1, Hailstone h2)
        => new[]
        {
            h2.Vel.Y - h1.Vel.Y, // RX
            h1.Vel.X - h2.Vel.X, // RY
            decimal.Zero, // RZ
            h1.Pos.Y - h2.Pos.Y, // RVX
            h2.Pos.X - h1.Pos.X, // RVY
            decimal.Zero, // RVZ
            h1.Pos.Y * h1.Vel.X - h1.Pos.X * h1.Vel.Y - h2.Pos.Y * h2.Vel.X + h2.Pos.X * h2.Vel.Y
        };

    // ReSharper disable once InconsistentNaming
    // RX(HVZ'-HVZ) + RZ(HVX-HVX') + RVX(HZ-HZ') + RVZ(HX'-HX) = HZ*HVX - HX*HVZ - HZ'*HVX' + HX'*HVZ'
    private static decimal[] ComposeXZ(Hailstone h1, Hailstone h2)
        => new[]
        {
            h2.Vel.Z - h1.Vel.Z, // RX
            decimal.Zero, // RY
            h1.Vel.X - h2.Vel.X, // RZ
            h1.Pos.Z - h2.Pos.Z, // RVX
            decimal.Zero, // RVY
            h2.Pos.X - h1.Pos.X, // RVZ
            h1.Pos.Z * h1.Vel.X - h1.Pos.X * h1.Vel.Z - h2.Pos.Z * h2.Vel.X + h2.Pos.X * h2.Vel.Z
        };

    // ReSharper disable once InconsistentNaming
    // RY(HVZ'-HVZ) + RZ(HVY-HVY') + RVY(HZ-HZ') + RVZ(HY'-HY) = HZ*HVY - HY*HVZ - HZ'*HVY' + HY'*HVZ'
    private static decimal[] ComposeYZ(Hailstone h1, Hailstone h2)
        => new[]
        {
            decimal.Zero, // RX
            h2.Vel.Z - h1.Vel.Z, // RY
            h1.Vel.Y - h2.Vel.Y, // RZ
            decimal.Zero, // RVX
            h1.Pos.Z - h2.Pos.Z, // RVY
            h2.Pos.Y - h1.Pos.Y, // RVZ
            h1.Pos.Z * h1.Vel.Y - h1.Pos.Y * h1.Vel.Z - h2.Pos.Z * h2.Vel.Y + h2.Pos.Y * h2.Vel.Z
        };

    private static decimal[] SolveMatrix(decimal[][] matrix)
    {
        for (var start = 0; start < matrix[0].Length - 1; start++)
        {
            PartialPivot(matrix, start);
            GaussianElimination(matrix, start);
        }

        return BackSubstitution(matrix);
    }

    private static void PartialPivot(decimal[][] matrix, int start)
    {
        // find the highest coefficient in the current column and use partial pivot
        // https://en.wikipedia.org/wiki/Pivot_element#Partial_and_complete_pivoting
        var pivotIndex = start;

        for (var i = start + 1; i < matrix.Length; i++)
        {
            if (Math.Abs(matrix[i][start]) > Math.Abs(matrix[pivotIndex][start]))
            {
                pivotIndex = i;
            }
        }

        if (pivotIndex == start)
        {
            return;
        }

        (matrix[pivotIndex], matrix[start]) = (matrix[start], matrix[pivotIndex]);
    }

    private static void GaussianElimination(decimal[][] matrix, int start)
    {
        for (var i = start + 1; i < matrix.Length; i++)
        {
            var factor = matrix[i][start] / matrix[start][start];

            for (var j = start; j < matrix[0].Length; j++)
            {
                matrix[i][j] -= factor * matrix[start][j];
            }
        }
    }
    
    private static decimal[] BackSubstitution(decimal[][] matrix)
    {
        // https://en.wikipedia.org/wiki/Triangular_matrix#Forward_and_back_substitution
        var result = new decimal[matrix[0].Length - 1];

        for (var i = matrix.Length - 1; i >= 0; i--)
        {
            var solved = decimal.Zero;

            for (var j = i + 1; j < matrix[0].Length - 1; j++)
            {
                solved += matrix[i][j] * result[j];
            }

            result[i] = (matrix[i][matrix[0].Length - 1] - solved) / matrix[i][i];
        }

        return result;
    }

    private static (T, T)[] GetPairs<T>(T[] source)
        => source
            .SelectMany((a, i) => source.Skip(i + 1).Select(b => (a, b)))
            .ToArray();

    private static Hailstone[] ParseHailstones(string input)
        => input.Split('\n')
            .Select(ParseHailstone)
            .ToArray();

    private static Hailstone ParseHailstone(string source)
    {
        var v = source.Split(new[] {'@', ','}, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(long.Parse)
            .ToArray();

        return new Hailstone(new Vec(v[0], v[1], v[2]), new Vec(v[3], v[4], v[5]));
    }

    private sealed record Hailstone(Vec Pos, Vec Vel);

    private sealed record Vec(long X, long Y, long Z);
}
