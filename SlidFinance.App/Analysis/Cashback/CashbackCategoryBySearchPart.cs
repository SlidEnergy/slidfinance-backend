using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlidFinance.App.Analysis
{
	public class CashbackCategoryBySearchPart
	{
		public string SearchPart;
		public int Id;
		public string Title;
		public int TariffId;
		public CashbackCategoryType Type;
	}
}
