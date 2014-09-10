using System;
using System.Collections.Generic;

namespace Karcero.Engine.Helpers
{
    public static class StaticTimer
    {
        private static readonly Dictionary<string, DateTime> mTimers = new Dictionary<string, DateTime>();
        private static DateTime mStartDate;
        public static Dictionary<string, double> Results = new Dictionary<string, double>();

        public static void StartMeasure(string key)
        {
            if (mTimers.Count == 0)
            {
                mStartDate = DateTime.Now;
            }
            mTimers[key] = DateTime.Now;
        }

        public static void EndMeasure(string key)
        {
            if (!Results.ContainsKey(key))
            {
                Results[key] = 0;
            }
            Results[key] += DateTime.Now.Subtract(mTimers[key]).TotalSeconds;
            mTimers[key] = DateTime.MinValue;
        }

        public static void WriteResults(int iterations)
        {
            foreach (var kvp in Results)
            {
                Console.WriteLine("{0}: {1} seconds on average", kvp.Key, (kvp.Value / iterations));
            }
            Console.WriteLine("Total time: {0} seconds on average", DateTime.Now.Subtract(mStartDate).TotalSeconds / iterations );
        }
    }
}
