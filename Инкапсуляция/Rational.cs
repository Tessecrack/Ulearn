using System;
namespace Incapsulation.RationalNumbers
{
    public class Rational
    {
        public int Numerator { get; set; }

        public int Denominator { get; set; }

        public bool IsNan
        {
            get => Denominator == 0;
        }

        public Rational(int numerator, int denominator = 1)
        {
            if (denominator != 0)
                Reduce(ref numerator, ref denominator);
            Numerator = numerator;
            Denominator = denominator;
            
        }
        private void ReduceTwo(ref int numerator, ref int denominator)
        {
            if (numerator % 2 == 0 && denominator % 2 == 0)
            {
                numerator /= 2;
                denominator /= 2;
            }
        }
        private void ReduceThree(ref int numerator, ref int denominator)
        {
            if (numerator % 3 == 0 && denominator % 3 == 0)
            {
                numerator /= 3;
                denominator /= 3;
            }
        }
        private void ReduceFive(ref int numerator, ref int denominator)
        {
            if (numerator % 5 == 0 && denominator % 5 == 0)
            {
                numerator /= 5;
                denominator /= 5;
            }
        }
        private void Reduce(ref int numerator, ref int denominator)
        {
            if (numerator < 0 && denominator < 0 || denominator < 0) 
            {
                numerator *= -1; denominator *= -1;
            }
            while (numerator % 2 == 0 && denominator % 2 == 0 || 
                numerator % 3 == 0 && denominator % 3 == 0 ||
                numerator % 5 == 0 && denominator % 5 == 0 )
            {
                ReduceTwo(ref numerator, ref denominator);
                ReduceThree(ref numerator, ref denominator);
                ReduceFive(ref numerator, ref denominator);
            }
        }

        public static Rational operator +(Rational rat1, Rational rat2)
        {
            if (rat1.IsNan || rat2.IsNan) return new Rational(0, 0);
            int numerator, denominator;
            if (rat1.Denominator != rat2.Denominator)
            {
                denominator = rat1.Denominator * rat2.Denominator;
                numerator = rat1.Numerator * (denominator / rat1.Denominator) + rat2.Numerator * (denominator / rat2.Denominator);
            }
            else
            {
                denominator = rat1.Denominator;
                numerator = rat1.Numerator + rat2.Numerator;
            }
            return new Rational(numerator, denominator);
        }
        public static Rational operator -(Rational rat1, Rational rat2)
        {
            if (rat1.IsNan || rat2.IsNan) return new Rational(0, 0);
            int numerator, denominator;
            if (rat1.Denominator != rat2.Denominator)
            {
                denominator = rat1.Denominator * rat2.Denominator;
                numerator = rat1.Numerator * (denominator / rat1.Denominator) - rat2.Numerator * (denominator / rat2.Denominator);
            }
            else
            {
                denominator = rat1.Denominator;
                numerator = rat1.Numerator + rat2.Numerator;
            }
            return new Rational(numerator, denominator);
        }
        public static Rational operator *(Rational rat1, Rational rat2)
        {
            var numerator = rat1.Numerator * rat2.Numerator;
            var denominator = rat1.Denominator * rat2.Denominator;
            return new Rational(numerator, denominator);
        }
        public static Rational operator /(Rational rat1, Rational rat2)
        {
            if (rat1.IsNan || rat2.IsNan)
                return new Rational(0, 0);
            var numerator = rat1.Numerator * rat2.Denominator;
            var denominator = rat1.Denominator * rat2.Numerator;

            if (rat2.Numerator < 0)
            {
                numerator *= -1;
                denominator *= -1;
            }
            Rational rat = new Rational(numerator, denominator);
            return rat;
        }

        public static implicit operator double(Rational rat) => rat.Denominator == 0 ?
            double.NaN :
            (double)((double)rat.Numerator / (double)rat.Denominator);
        public static implicit operator Rational(int val) => new Rational(numerator: val);
        public static implicit operator int(Rational rat) => rat.Numerator % rat.Denominator == 0 ?
            rat.Numerator / rat.Denominator :
            throw new System.Exception();
    }
}
