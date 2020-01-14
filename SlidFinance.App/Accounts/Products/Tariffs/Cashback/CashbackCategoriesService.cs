using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class CashbackCategoriesService : ICashbackCategoriesService
	{
		private IApplicationDbContext _context;

		public CashbackCategoriesService(IApplicationDbContext context)
		{
			_context = context;
		}

		public Task<List<CashbackCategory>> GetListWithAccessCheckAsync(string userId, int tariffId) => _context.GetCashbackCategoriesWithAccessCheckAsync(userId, tariffId);
	}
}
