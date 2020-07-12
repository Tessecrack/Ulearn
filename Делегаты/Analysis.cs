using System;
using System.Collections.Generic;
using System.Linq;

namespace Delegates.PairsAnalysis
{
    public static class Analysis
    {
        public static int FindMaxPeriodIndex(params DateTime[] data)
            => data
            .Pairs()
            .Select((dataTime) => (dataTime.Item2 - dataTime.Item1).TotalSeconds).MaxIndex();
        public static double FindAverageRelativeDifference(params double[] data)
            => new AverageDifferenceFinder().Analyze(data);
        public static int MaxIndex<T>(this IEnumerable<T> data) where T : IComparable
        {
            int index = 0, i = 0;
            var max = default(T);
            bool first = true;
            foreach (var item in data)
            {
                if (first)
                {
                    max = item;
                    first = false;
                }
                if (item.CompareTo(max) == 1)
                {
                    index = i;
                    max = item;
                }
                i++;
            }
            if (first) throw new ArgumentException();
            return index;
        }

        public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> data)
        {
            bool first = true;
            T prev = default;
            foreach (var item in data)
            {
                if (first)
                {
                    first = false;
                    prev = item;
                    continue;
                }
                yield return Tuple.Create(prev, item);
                prev = item;
            }
            if (first) throw new ArgumentException();
        }
    }
}
