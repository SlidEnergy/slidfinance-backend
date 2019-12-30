using SlidFinance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public interface IMerchantService
	{
		Task<Models.Merchant> GetByIdWithAccessCheckAsync(string userId, int id);
		Task<Models.Merchant> AddAsync(Merchant merchant);
		Task<List<Models.Merchant>> GetListAsync();
		Task<List<Models.Merchant>> GetListWithAccessCheckAsync(string userId);
		Task<Models.Merchant> EditMerchant(string userId, Models.Merchant merchant);
	}
}