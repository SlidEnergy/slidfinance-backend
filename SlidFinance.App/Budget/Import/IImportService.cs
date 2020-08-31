using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IImportService
	{
		Task<int> Import(string userId, int accountId, float? balance, Transaction[] transactions);
	}
}