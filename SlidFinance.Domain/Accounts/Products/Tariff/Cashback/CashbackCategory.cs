using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SlidFinance.Domain
{
	public class CashbackCategory
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public int TariffId { get; set; }
		[Required]
		public virtual ProductTariff Tariff { get; set; }
	}
}
