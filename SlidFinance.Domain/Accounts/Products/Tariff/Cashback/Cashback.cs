using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SlidFinance.Domain
{
	public class Cashback
	{
		public int Id { get; set; }

		public int CategoryId { get; set; }
		[Required]
		public virtual CashbackCategory Category { get; set; }

		public float Percent { get; set; }
	}
}
