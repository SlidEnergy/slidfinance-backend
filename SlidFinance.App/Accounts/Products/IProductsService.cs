using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IProductsService
	{
		Task<Product> Add(string userId, Product product);
		Task Delete(string userId, int productId);
		Task<Product> Edit(string userId, Product product);
		Task<Product> GetByIdWithAccessCheck(string userId, int id);
		Task<List<Product>> GetListWithAccessCheckAsync(string userId);
	}
}