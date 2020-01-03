using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SlidFinance.WebApi.Dto
{
	public class Product
	{
		public int Id { get; set; }

		public string Title { get; set; }

		public int? BankId { get; set; }

		public ProductType Type { get; set; }

		public string Image { get; set; }

		public bool IsPublic { get; set; }

		public bool Approved { get; set; }
	}
}
