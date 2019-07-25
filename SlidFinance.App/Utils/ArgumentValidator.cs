using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App.Utils
{
	public static class ArgumentValidator
	{
		public static void ValidateId(int id)
		{
			if (id <= 0)
				throw new ArgumentOutOfRangeException();
		}

		public static void ValidatePeriod(DateTime startDate, DateTime endDate)
		{
			if (startDate >= endDate)
				throw new ArgumentOutOfRangeException();
		}

		public static void ValidatePeriodAllowEqual(DateTime startDate, DateTime endDate)
		{
			if (startDate > endDate)
				throw new ArgumentOutOfRangeException();
		}
	}
}
