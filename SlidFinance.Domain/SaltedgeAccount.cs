using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SlidFinance.Domain
{
	public class SaltedgeAccount
	{
		public string UserId { get; set; }

		[Required]
		public virtual ApplicationUser User { get; set; }

		public string CustomerId { get; set; }
	}
}
