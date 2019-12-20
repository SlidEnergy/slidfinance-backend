using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class MerchantService : IMerchantService
	{
		private readonly IApplicationDbContext _context;

		public MerchantService(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<Models.Merchant>> GetListAsync()
		{
			return await _context.Merchants.ToListAsync();
		}

		public async Task<Models.Merchant> AddAsync(Models.Merchant merchant)
		{
			_context.Merchants.Add(merchant);
			await _context.SaveChangesAsync();

			return merchant;
		}
	}
}
