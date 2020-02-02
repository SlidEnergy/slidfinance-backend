using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class ProductsService : IProductsService
	{
		private IApplicationDbContext _context;

		public ProductsService(IApplicationDbContext context)
		{
			_context = context;
		}

		public Task<Product> GetByIdWithAccessCheck(string userId, int id) => _context.GetProductByIdWithAccessCheck(userId, id);

		public Task<List<Product>> GetListWithAccessCheckAsync(string userId) => _context.GetProductListWithAccessCheckAsync(userId);

		public async Task<Product> Add(string userId, Product product)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

			_context.Products.Add(product);
			await _context.SaveChangesAsync();
			_context.TrusteeProducts.Add(new TrusteeProduct(user, product));
			await _context.SaveChangesAsync();

			return product;
		}

		public async Task<Product> Edit(string userId, Product product)
		{
			var model = await _context.GetProductByIdWithAccessCheck(userId, product.Id);

			model.Title = product.Title;
			model.Image = product.Image;
			model.IsPublic = product.IsPublic;
			model.Approved = product.Approved;
			model.Type = product.Type;

			_context.Products.Update(model);
			await _context.SaveChangesAsync();

			return model;
		}

		public async Task Delete(string userId, int id)
		{
			var product = await _context.GetProductByIdWithAccessCheck(userId, id);

			if (product == null) 
				return;

			_context.Products.Remove(product);
			await _context.SaveChangesAsync();
		}
	}
}
