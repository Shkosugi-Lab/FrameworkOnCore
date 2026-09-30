using FrameworkOnCore.Parity;

namespace FrameworkOnCore.DrawingParity
{
    /// <summary>This suite's cases (no database).</summary>
    public static class DrawingCases
    {
        static DrawingCases() => Probe.Custom = Describe.Custom;

        public static readonly Runner Runner = new Runner(typeof(DrawingCases).Assembly);
    }
}
