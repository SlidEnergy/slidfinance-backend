using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlidFinance.App.Analysis
{
	public class CashbackCategoryBySearchPartWithCashbackPercent
	{
		public string SearchPart;
		public int Id;
		public string CategoryTitle;
		public int TariffId;
		public CashbackCategoryType Type;
		public float Percent;
	}
}
