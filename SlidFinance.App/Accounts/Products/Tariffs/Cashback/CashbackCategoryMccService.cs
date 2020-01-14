using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class CashbackCategoryMccService : ICashbackCategoryMccService
	{
		private IApplicationDbContext _context;

		public CashbackCategoryMccService(IApplicationDbContext context)
		{
			_context = context;
		}

		public Task<List<CashbackCategoryMcc>> GetListWithAccessCheckAsync(string userId, int categoryId) => _context.GetCashbackCategoryMccWithAccessCheckAsync(userId, categoryId);

		public async Task<CashbackCategoryMcc> Add(string userId, CashbackCategoryMcc cashbackMcc)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

			_context.CashbackCategoryMcc.Add(cashbackMcc);
			await _context.SaveChangesAsync();

			return cashbackMcc;
		}
	}
}
