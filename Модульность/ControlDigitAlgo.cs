using NUnit.Framework;
using System;

namespace SRP.ControlDigit
{
	public static class ControlDigitAlgo
	{
		public static int Upc(long number)
		{
			return number.GetDigitalSum(3).GetControlNumber(10);
		}

		public static int Isbn10(long number)
		{
			var controlDigit = number.GetDigitalSum(2, false).GetControlNumber(11);
			return controlDigit == 10 ? 'X' : char.Parse(controlDigit.ToString());
		}

		public static int Isbn13(long number)
		{
			return number.GetDigitalSum(1).GetControlNumber(10);
		}
	}
	
    public static class Extensions
    {
	public static int GetDigitalSum(this long number, int factor, bool check = true)
        {
		var sum = 0;
		do
		{
			var value = (int)(number % 10);
			sum += factor * value;
			factor = check ? 4 - factor : ++factor;
			number /= 10;
		}
		while (number > 0);
		return sum;
        }

      public static int GetControlNumber(this int number, int coef)
			=> number % coef == 0 ? number % coef : coef - number % coef;
	}
}
