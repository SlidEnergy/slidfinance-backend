using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SlidFinance.Domain
{
	public class CashbackCondition
	{
		public int Id { get; set; }

		public int CashbackId { get; set; }
		[Required]
		public virtual Cashback Cashback { get; set; }

		public CashbackConditionType Type { get; set; }
	}
}
