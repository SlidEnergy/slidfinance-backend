using SlidFinance.Domain;
using System;

namespace SlidFinance.WebApi.Dto
{
	public class BankAccount
	{
		public int Id { get; set; }

		public string Code { get; set; }

		public float Balance { get; set; }

		public float CreditLimit { get; set; }

		public string Title { get; set; }

		public int? BankId { get; set; }

		public int? SelectedTariffId { get; set; }

		public int? ProductId { get; set; }

		public ProductType Type { get; set; }

		public DateTime? Opened { get; set; }

		public string SaltedgeBankAccountId { get; set; }
	}
}
