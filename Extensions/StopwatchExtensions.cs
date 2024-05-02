using System.Diagnostics;

namespace FolderSizeSearcher.Extensions
{
    public static class StopwatchExtensions
    {
        public static string GetElapsedTimeText(this Stopwatch stopwatch)
        {
            var totalTime = (double)stopwatch.ElapsedMilliseconds;
            var timeType = "milliseconds";

            if (totalTime > 999d)
            {
                totalTime /= 1000d;
                timeType = "seconds";
            }

            if (totalTime > 59d)
            {
                totalTime /= 60d;
                timeType = "minutes";
            }

            return $"{totalTime:N3} {timeType}";
        }
    }
}
