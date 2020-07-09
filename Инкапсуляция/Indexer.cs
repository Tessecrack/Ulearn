using System;

namespace Incapsulation.Weights
{
	public class Indexer
    {
        private readonly int startIndex;
        public int Length { get; set; }
        private readonly double[] array;
        public Indexer(double[] array, int start, int length)
        {
            if (start < 0 
                || start > array.Length 
                || length < 0 
                || length > array.Length 
                || start + length > array.Length)
                throw new ArgumentException();
            this.array = array;
            startIndex = start;
            this.Length = length;
        }

        public double this[int index]
        {
            get => index >= 0 && index < array.Length && index < Length ? array[startIndex + index] 
				: throw new IndexOutOfRangeException();
            set
            {
                if (index >= 0 && index < Length)
                    array[startIndex + index] = value;
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
        }
    }
}
