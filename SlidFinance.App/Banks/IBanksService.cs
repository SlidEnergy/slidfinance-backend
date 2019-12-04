using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IBanksService
	{
		Task<Bank> AddBank(string userId, string title);
		Task DeleteBank(string userId, int bankId);
		Task<Bank> EditBank(string userId, int bankId, string title);
		Task<List<Bank>> GetListWithAccessCheckAsync(string userId);
	}
}