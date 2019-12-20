using SlidFinance.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public interface IMerchantService
	{
		Task<Merchant> AddAsync(Merchant merchant);
		Task<List<Merchant>> GetListAsync();
	}
}