using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SlidFinance.Domain
{
	[Table("DepositAccounts")]
	public class DepositAccount: BankAccount
	{
		public DateTime PeriodStartDate { get; set; }

		public int Duration { get; set; }
	}
}
