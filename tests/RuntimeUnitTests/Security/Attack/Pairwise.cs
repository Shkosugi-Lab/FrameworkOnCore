namespace FrameworkOnCore.Runtime.Tests.Security;

/// <summary>
/// Cases of several axes where every pair of two axes' values is in a case (all-pairs: a defect two of them make together,
/// as path info and a denied type, or a rewrite and a hidden segment, is found by some case), made the same each time
/// (greedily, in the axes' order: IIS's answers are recorded for them).
/// </summary>
static class Pairwise
{
    public static List<int[]> Of(params int[] sizes)
    {
        var uncovered = new HashSet<(int A, int X, int B, int Y)>();
        for (var a = 0; a < sizes.Length; a++)
            for (var b = a + 1; b < sizes.Length; b++)
                for (var x = 0; x < sizes[a]; x++)
                    for (var y = 0; y < sizes[b]; y++)
                        uncovered.Add((a, x, b, y));
        var cases = new List<int[]>();
        while (uncovered.Count > 0)
        {
            // From the first pair not covered yet, each other axis the value that covers the most pairs not covered.
            var (fa, fx, fb, fy) = uncovered.OrderBy(p => p.A).ThenBy(p => p.B).ThenBy(p => p.X).ThenBy(p => p.Y).First();
            var values = Enumerable.Repeat(-1, sizes.Length).ToArray();
            values[fa] = fx;
            values[fb] = fy;
            for (var axis = 0; axis < sizes.Length; axis++)
            {
                if (values[axis] >= 0) continue;
                var best = 0;
                var bestCount = -1;
                for (var value = 0; value < sizes[axis]; value++)
                {
                    var count = 0;
                    for (var other = 0; other < sizes.Length; other++)
                    {
                        if (other == axis || values[other] < 0) continue;
                        var pair = other < axis ? (other, values[other], axis, value) : (axis, value, other, values[other]);
                        if (uncovered.Contains(pair)) count++;
                    }
                    if (count > bestCount) (best, bestCount) = (value, count);
                }
                values[axis] = best;
            }
            for (var a = 0; a < sizes.Length; a++)
                for (var b = a + 1; b < sizes.Length; b++)
                    uncovered.Remove((a, values[a], b, values[b]));
            cases.Add(values);
        }
        return cases;
    }
}
