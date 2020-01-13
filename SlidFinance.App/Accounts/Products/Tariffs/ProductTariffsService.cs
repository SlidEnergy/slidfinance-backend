using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class ProductTariffsService: IProductTariffsService
	{
		private IApplicationDbContext _context;

		public ProductTariffsService(IApplicationDbContext context)
		{
			_context = context;
		}

		public Task<ProductTariff> GetByIdWithAccessCheck(string userId, int id) => _context.GetProductTariffByIdWithAccessCheck(userId, id);

		public Task<List<ProductTariff>> GetListWithAccessCheckAsync(string userId, int productId) => _context.GetProductTariffsWithAccessCheckAsync(userId, productId);

		public async Task<ProductTariff> Add(string userId, ProductTariff tariff)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

			_context.Tariffs.Add(tariff);
			await _context.SaveChangesAsync();

			return tariff;
		}

		public async Task<ProductTariff> Edit(string userId, ProductTariff tariff)
		{
			var model = await _context.GetProductTariffByIdWithAccessCheck(userId, tariff.Id);

			model.Title = tariff.Title;
			model.Type = tariff.Type;

			_context.Tariffs.Update(model);
			await _context.SaveChangesAsync();

			return model;
		}

		public async Task Delete(string userId, int id)
		{
			var tariff = await _context.GetProductByIdWithAccessCheck(userId, id);

			if (tariff == null)
				return;

			_context.Products.Remove(tariff);
			await _context.SaveChangesAsync();
		}
	}
}
