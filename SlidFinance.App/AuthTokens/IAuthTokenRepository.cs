using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.App
{
    public interface IAuthTokenRepository: IRepository<AuthToken, int>
	{
		Task<AuthToken> FindAnyToken(string token);
    }
}
