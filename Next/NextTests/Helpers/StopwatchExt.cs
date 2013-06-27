using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NextTests.Helpers
{
    public static class StopwatchExt
    {
        public static string GetTimeString(this Stopwatch stopwatch, int numberofDigits = 1)
        {
            double s = stopwatch.ElapsedTicks / (double)Stopwatch.Frequency;
            if (s > 1)
                return Math.Round(s, numberofDigits) + " s";
            if (s > 1e-3)
                return Math.Round(1e3 * s, numberofDigits) + " ms";
            if (s > 1e-6)
                return Math.Round(1e6 * s, numberofDigits) + " µs";
            if (s > 1e-9)
                return Math.Round(1e9 * s, numberofDigits) + " ns";
            return stopwatch.ElapsedTicks + " ticks";
        }
    }
}
