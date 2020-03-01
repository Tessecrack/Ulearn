using System;
namespace Incapsulation.Weights
{
	public class Indexer
    {
        private readonly double[] array;
        public int Length { get; }
        public int begin;
        public Indexer(double[] range, int start, int length)
        {
            if (start < 0 || length < 0 ||
                range.Length < length ||
                start > range.Length ||
                start + length == range.Length + 1) throw new ArgumentException();
            array = range;
            Length = length;
            begin = start;
        }
        public double this[int index]
        {
            get
            {
                if (index >= 0 && index < Length)
                    return this.array[begin + index];
                else throw new IndexOutOfRangeException();
            }
            set
            {
                if (index >= 0 && index < Length)
                    this.array[begin + index] = value;
                else throw new IndexOutOfRangeException();
            }
        }
    }
}
