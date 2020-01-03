using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IProductsService
	{
		Task<Product> AddProduct(string userId, Product product);
		Task DeleteProduct(string userId, int productId);
		Task<Product> EditProduct(string userId, Product product);
		Task<Product> GetByIdWithAccessCheck(string userId, int id);
		Task<List<Product>> GetListWithAccessCheckAsync(string userId);
	}
}