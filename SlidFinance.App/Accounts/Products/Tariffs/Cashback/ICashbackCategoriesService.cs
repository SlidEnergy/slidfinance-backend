using SlidFinance.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public interface ICashbackCategoriesService
	{
		Task<List<CashbackCategory>> GetListWithAccessCheckAsync(string userId, int tariffId);
		Task<CashbackCategory> Add(string userId, CashbackCategory category);
		Task<CashbackCategory> Edit(string userId, CashbackCategory category);
		Task Delete(string userId, int id);
	}
}