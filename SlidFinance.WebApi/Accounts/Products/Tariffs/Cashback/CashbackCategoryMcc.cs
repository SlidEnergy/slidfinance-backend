using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SlidFinance.WebApi.Dto
{ 
	public class CashbackCategoryMcc
	{
		public int Id { get; set; }

		public int CategoryId { get; set; }

		public int MccId { get; set; }
	}
}
