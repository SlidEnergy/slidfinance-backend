using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SlidFinance.Domain
{
	[Table("CardProduct")]
	public class CardProduct: Product
	{
		public PaymentSystemType PaymentSystemType { get; set; }
	}
}
