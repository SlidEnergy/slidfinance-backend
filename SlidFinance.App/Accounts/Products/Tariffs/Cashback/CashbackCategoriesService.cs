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

		public async Task<CashbackCategory> Add(string userId, CashbackCategory category)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

			_context.CashbackCategories.Add(category);
			await _context.SaveChangesAsync();

			return category;
		}

		public async Task<CashbackCategory> Edit(string userId, CashbackCategory category)
		{
			var model = await _context.GetCashbackCategoryByIdWithAccessCheck(userId, category.Id);

			model.Title = category.Title;

			_context.CashbackCategories.Update(model);
			await _context.SaveChangesAsync();

			return model;
		}
	}
}
