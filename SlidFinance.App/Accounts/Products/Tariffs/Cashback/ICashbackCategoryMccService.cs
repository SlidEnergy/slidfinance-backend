using SlidFinance.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public interface ICashbackCategoryMccService
	{
		Task<List<CashbackCategoryMcc>> GetListWithAccessCheckAsync(string userId, int categoryId);

		Task<CashbackCategoryMcc> Add(string userId, CashbackCategoryMcc cashbackMcc);
	}
}