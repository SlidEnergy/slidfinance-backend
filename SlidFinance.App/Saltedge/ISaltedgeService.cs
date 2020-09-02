using System.Collections.Generic;
using System.Threading.Tasks;
using SaltEdgeNetCore.Models.Connections;
using SlidFinance.Domain;

namespace SlidFinance.App.Saltedge
{
	public interface ISaltedgeService
	{
		Task<SaltedgeAccount> AddSaltedgeAccount(string userId, SaltedgeAccount saltedgeAccount);

		Task<IEnumerable<SaltedgeBankAccounts>> GetSaltedgeBankAccounts(string userId);

		Task<int> Import(string userId);
	}
}