using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SlidFinance.WebApi.Dto
{
	public class ProductTariff
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public int ProductId { get; set; }

		public ProductType Type { get; set; }
	}
}
