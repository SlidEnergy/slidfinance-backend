using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SlidFinance.Domain
{
	[Table("DepositAccounts")]
	public class DepositAccount: BankAccount
	{
		public int Duration { get; set; }
	}
}
