using System;

namespace SRP.ControlDigit
{
	public static class ControlDigitAlgo
	{
		public static int Upc(long number)
		{
			int sum = 0;
			int factor = 3;
			do
			{
				int digit = (int)(number % 10);
				sum += factor * digit;
				factor = 4 - factor;
				number /= 10;

			}
			while (number > 0);

			int result = sum % 10;
			if (result != 0)
				result = 10 - result;
			return result;
		}

		public static int Isbn10(long number)
		{
			throw new NotImplementedException();
		}

		public static int Isbn13(long number)
		{
			throw new NotImplementedException();
		}
	}
}
