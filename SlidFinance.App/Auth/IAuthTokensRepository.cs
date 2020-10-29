using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public interface IAuthTokensRepository: IRepository<AuthToken, int>
	{
		Task<AuthToken> FindAnyToken(string token);
	}
}
