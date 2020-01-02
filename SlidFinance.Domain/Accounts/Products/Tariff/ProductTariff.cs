using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SlidFinance.Domain
{
	public class ProductTariff
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public int ProductId { get; set; }
		[Required]
		public virtual Product Product { get; set; }

		public ProductType Type { get; set; }
	}
}
