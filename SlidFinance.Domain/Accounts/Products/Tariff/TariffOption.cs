using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SlidFinance.Domain
{
	public class TariffOption
	{
		public int Id { get; set; }

		public int TariffId { get; set; }

		[Required]
		public virtual ProductTariff Tariff{ get; set; }

		public TariffOptionType Type { get; set; }

		public float Cost { get; set; }

		public string Value { get; set; }
	}
}
