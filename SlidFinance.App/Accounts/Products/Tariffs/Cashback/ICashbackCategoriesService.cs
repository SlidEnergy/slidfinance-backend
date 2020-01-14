using SlidFinance.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public interface ICashbackCategoriesService
	{
		Task<List<CashbackCategory>> GetListWithAccessCheckAsync(string userId, int tariffId);
	}
}