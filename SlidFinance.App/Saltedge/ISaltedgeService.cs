using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App.Saltedge
{
	public interface ISaltedgeService
	{
		Task<SaltedgeAccount> AddSaltedgeAccount(string userId, SaltedgeAccount saltedgeAccount);
	}
}