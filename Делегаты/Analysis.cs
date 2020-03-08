using System;
using System.Collections.Generic;

namespace Delegates.PairsAnalysis
{
    public static class Analysis
    {
        public static int FindMaxPeriodIndex(params DateTime[] data)
            => data.Pairs().Select((data1, data2) => (data2 - data1).TotalSeconds).MaxIndex();

        public static double FindAverageRelativeDifference(params double[] data)
            => new AverageDifferenceFinder().Analyze(data);

        public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> data)
        {
            bool firstElement = true;
            bool empty = true;
            T previousElement = default(T);
            foreach (var element in data)
            {
                if (firstElement)
                {
                    previousElement = element;
                    firstElement = false;
                }
                else
                {
                    yield return Tuple.Create(previousElement, element);
                    previousElement = element;
                    empty = false;
                }
            }
            if (empty) throw new ArgumentException();
        }

        private static IEnumerable<TypeOut> Select<TypeIn, TypeOut>
            (this IEnumerable<Tuple<TypeIn, TypeIn>> pairs, Func<TypeIn, TypeIn, TypeOut> insect)
        {
            foreach (var element in pairs)
                yield return insect(element.Item1, element.Item2);
        }

        public static int MaxIndex<Type>(this IEnumerable<Type> data) where Type : IComparable
        {
            int index = 0, i = 0;
            var maxValue = default(Type);
            bool firstElement = true;
            bool empty = true;
            foreach (var element in data)
            {
                if (firstElement)
                { maxValue = element; firstElement = false; empty = false; }
                else
                {
                    if (element.CompareTo(maxValue) == 1)
                    { index = i; maxValue = element; }
                }
                i++;
            }
            if (empty) throw new ArgumentException();
            return index;
        }
    }
}
