using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SlidFinance.Domain
{ 
	public class CashbackCategoryMcc
	{
		public int Id { get; set; }

		public int CategoryId { get; set; }
		[Required]
		public virtual CashbackCategory Category { get; set; }

		public int MccId { get; set; }
		[Required]
		public virtual MccCategory Mcc { get; set; }
	}
}
