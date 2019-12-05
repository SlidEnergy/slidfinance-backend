using System;
using System.Collections.Generic;
using System.Text;

namespace SlidFinance.App
{
	public class RuleComparer : IEqualityComparer<GeneratedRule>
	{
		bool IEqualityComparer<GeneratedRule>.Equals(GeneratedRule x, GeneratedRule y)
		{
			return ((x.AccountId == null || x.AccountId.Equals(y.AccountId)) &&
					(string.IsNullOrEmpty(x.BankCategory) || x.BankCategory.Equals(y.BankCategory)) &&
					(string.IsNullOrEmpty(x.Description) || x.Description.Equals(y.Description)) &&
					(x.Mcc == null || x.Mcc.Equals(y.Mcc)));
		}

		int IEqualityComparer<GeneratedRule>.GetHashCode(GeneratedRule obj)
		{
			if (obj == null)
				return 0;

			return (obj.BankCategory ?? "").GetHashCode() * 1000 +
				   (obj.Description ?? "").GetHashCode() * 100 +
				   ((obj.AccountId ?? 0) + 1) * 10 +
				   obj.Mcc ?? 0;
		}
	}
}
