using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SlidFinance.WebApi.Dto
{
	public class CashbackCategory
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public int TariffId { get; set; }

		public float CashbackLimit { get; set; }

		public CashbackCategoryType Type { get; set; }
	}
}
