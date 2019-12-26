using SlidFinance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public interface IMerchantService
	{
		Task<Merchant> AddAsync(Merchant merchant);
		Task<List<Merchant>> GetListAsync();
		Task<List<Merchant>> GetListWithAccessCheckAsync(string userId);
		Task<Models.Merchant> EditMerchant(string userId, Models.Merchant merchant);
	}
}