using System;

namespace Incapsulation.RationalNumbers
{
    public class Rational
    {
        public int Numerator { get; set; }
        public int Denominator { get; set; }
        public bool IsNan => Denominator == 0;
        public Rational(int numerator, int denominator = 1)
        {
            Numerator = numerator;
            Denominator = denominator;
            if (!IsNan) FractionReduction(this);
            if (Numerator == 0 && Denominator != 0) Denominator = 1;
            FromNegativeToPositive();
        }
		
        private void FromNegativeToPositive()
        {
            if (Numerator < 0 && Denominator < 0 || Numerator > 0 && Denominator < 0) 
            {
                Numerator *= (-1);
                Denominator *= (-1);
            }
        }


        public static Rational operator +(Rational ratioFirst, Rational ratioSecond)
        {
            int generalDenominator = 0, generalNumerator = 1;
            if (!ratioFirst.IsNan && !ratioSecond.IsNan)
            {
                generalDenominator = ratioFirst.Denominator * ratioSecond.Denominator;
                generalNumerator = (generalDenominator / ratioFirst.Denominator) * ratioFirst.Numerator
                    + (generalDenominator / ratioSecond.Denominator) * ratioSecond.Numerator;
            }
            var result = new Rational(generalNumerator, generalDenominator);
            FractionReduction(result);
            return result;
        }

        public static Rational operator -(Rational ratioFirst, Rational ratioSecond)
        {
            int generalDenominator = 0, generalNumerator = 1;
            if (!ratioFirst.IsNan && !ratioSecond.IsNan)
            {
                generalDenominator = ratioFirst.Denominator * ratioSecond.Denominator;
                generalNumerator = (generalDenominator / ratioFirst.Denominator) * ratioFirst.Numerator
                    - (generalDenominator / ratioSecond.Denominator) * ratioSecond.Numerator;
            }
            var result = new Rational(generalNumerator, generalDenominator);
            FractionReduction(result);
            return result;
        }

        public static Rational operator *(Rational ratioFirst, Rational ratioSecond)
        {
            var result = new Rational(ratioFirst.Numerator * ratioSecond.Numerator,
                ratioFirst.Denominator * ratioSecond.Denominator);
            FractionReduction(result);
            return result;
        }

        public static Rational operator /(Rational ratioFirst, Rational ratioSecond)
        {
            int generalDenominator = 0, generalNumerator = 1;
            if (!ratioFirst.IsNan && !ratioSecond.IsNan)
            {
                generalDenominator = ratioFirst.Denominator * ratioSecond.Numerator;
                generalNumerator = ratioFirst.Numerator * ratioSecond.Denominator;
            }
            var result = new Rational(generalNumerator, generalDenominator);
            FractionReduction(result);
            return result;
        }

        public static implicit operator double(Rational ratio)
            => ratio.IsNan ? double.NaN : (double)((double)ratio.Numerator / (double)ratio.Denominator);

        public static implicit operator Rational(int value) => new Rational(value);

        public static implicit operator int(Rational ratio) => ratio.Numerator % ratio.Denominator == 0 ? 
            ratio.Numerator / ratio.Denominator :
            throw new System.Exception();

        private static void FractionReduction(Rational number)
        {
            if (number.IsNan) return;
            ValueReduction(number, 2);
            ValueReduction(number, 3);
            ValueReduction(number, 5);
            ValueReduction(number, 7);
        }

        private static void ValueReduction(Rational number, int value)
        {
            if (number.Numerator % value == 0 && number.Denominator % value == 0)
            {
                number.Numerator /= value;
                number.Denominator /= value;
                FractionReduction(number);
            }
        }
    }
}
