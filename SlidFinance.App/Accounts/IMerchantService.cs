using SlidFinance.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public interface IMerchantService
	{
		Task<Merchant> GetByIdWithAccessCheckAsync(string userId, int id);
		Task<Merchant> AddAsync(Merchant merchant);
		Task<List<Merchant>> GetListAsync();
		Task<List<Merchant>> GetListWithAccessCheckAsync(string userId);
		Task<Merchant> EditMerchant(string userId, Merchant merchant);
	}
}