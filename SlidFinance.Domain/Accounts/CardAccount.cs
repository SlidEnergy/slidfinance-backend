using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SlidFinance.Domain
{
	[Table("CardAccounts")]
	public class CardAccount: BankAccount
	{
		public float CreditLimit { get; set; }

		public override float OwnFunds => Balance - CreditLimit;
	}
}
