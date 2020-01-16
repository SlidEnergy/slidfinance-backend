using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlidFinance.App.Analysis
{
	public class WhichCardToPay
	{
		public string SearchPart;
		public string BankTitle;
		public string AccountTitle;
		public string CategoryTitle;
		public float Percent;

		public override string ToString()
		{
			var title = string.Format("{0}: {1} от {2} ({3}) - {4:P2}%", SearchPart, AccountTitle, BankTitle, CategoryTitle, Percent);

			return title;
		}
	}
}
