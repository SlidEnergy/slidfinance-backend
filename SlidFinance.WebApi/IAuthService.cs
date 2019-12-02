using System.Threading.Tasks;
using SlidFinance.App;

namespace SlidFinance.WebApi
{
	public interface IAuthService
	{
		Task<TokensCortage> CheckCredentialsAndGetToken(string email, string password);
	}
}