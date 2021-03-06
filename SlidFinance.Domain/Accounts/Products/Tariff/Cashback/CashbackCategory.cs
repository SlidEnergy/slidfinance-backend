﻿using Newtonsoft.Json;
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

		public string Description { get; set; }

		public int TariffId { get; set; }
		[JsonIgnore]
		public virtual ProductTariff Tariff { get; set; }

		public float CashbackLimit { get; set; }

		public CashbackCategoryType Type { get; set; }
	}
}
