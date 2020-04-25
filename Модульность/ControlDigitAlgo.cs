// Вставьте сюда финальное содержимое файла ControlDigitAlgo.cs
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace SRP.ControlDigit
{
    public static class ControlDigitAlgo
    {
        public static int Upc(long number)
            => number.GetDigitsOfNumber().GetWeightnessSum("UPC", 3).GetControlDigit(10); // 3

        public static char Isbn10(long number)
        {
            var res = number.GetDigitsOfNumber().GetWeightnessSum("ISBN10", 2).GetControlDigit(11);
            return res == 10 ? 'X' : char.Parse(res.ToString());
        }

        public static int Isbn13(long number) // 1
            => number.GetDigitsOfNumber().GetWeightnessSum("ISBN13", 1).GetControlDigit(10);
    }

    public static class Extensions
    {
        public static List<int> GetDigitsOfNumber(this long number)
        {
            var list = new List<int>();
            do
            {
                var digit = (int)(number % 10);
                list.Add(digit);
                number /= 10;
            } while (number > 0);
            return list;
        }

        public static int GetWeightnessSum(this List<int> digitOfNumbers,
                                           string nameCheckDigit, int coef)
        {
            var sum = 0;
            foreach (var digit in digitOfNumbers)
            {
                sum += coef * digit;
                if (nameCheckDigit == "UPC" || nameCheckDigit == "ISBN13") coef = 4 - coef;
                else coef++;
            }
            return sum;
        }

        public static int GetControlDigit(this int number, int coef) 
			=> number % coef != 0 ? coef -  number % coef :  number % coef;
    }
}


