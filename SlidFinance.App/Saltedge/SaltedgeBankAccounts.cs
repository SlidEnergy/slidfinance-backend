using System;
using System.Collections.Generic;
using System.Text;
using SaltEdgeNetCore.Models.Account;
using SaltEdgeNetCore.Models.Connections;

namespace SlidFinance.App.Saltedge
{
	public class SaltedgeBankAccounts
	{
		public SeConnection Connection { get; set; }

		public IEnumerable<SeAccount> Accounts { get; set; }
	}
}
