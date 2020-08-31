using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SlidFinance.Domain
{
	public class BankAccount : IUniqueObject
	{
		public int Id { get; set; }

		public string Code { get; set; }

		public float Balance { get; set; }

		public float CreditLimit { get; set; }

		public virtual float OwnFunds => Balance - CreditLimit;

		[Required]
		public string Title { get; set; }

		public int? BankId { get; set; }
		public virtual Bank Bank { get; set; }

		public int? SelectedTariffId { get; set; }
		public virtual ProductTariff SelectedTariff { get; set; }

		public int? ProductId { get; set; }
		public virtual Product Product { get; set; }

		public ProductType Type { get; set; }

		public DateTime? Opened { get; set; }

		public string SaltedgeBankAccountId { get; set; }

		public BankAccount() { }

		public BankAccount(Bank bank, string title, string code, float balance, float creditLimit)
		{
			Bank = bank;
			Title = title;
			Code = code;
			Balance = balance;
			CreditLimit = creditLimit;

		}

		public void Update(string title, string code, float balance, float creditLimit)
		{
			Title = title;
			Code = code;
			Balance = balance;
			CreditLimit = creditLimit;

		}
	}
}
