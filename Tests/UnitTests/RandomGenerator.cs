using System;
using System.Collections.Generic;
using System.Text;

namespace SlidFinance.UnitTests
{
	public static class RandomGenerator
	{
		public static int GenerateMcc()
		{
			var r = new Random();
			return r.Next(1000, 9999);
		}
	}
}
