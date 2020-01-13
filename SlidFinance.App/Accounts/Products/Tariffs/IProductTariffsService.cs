using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IProductTariffsService
	{
		Task<ProductTariff> Add(string userId, ProductTariff tariff);
		Task Delete(string userId, int id);
		Task<ProductTariff> Edit(string userId, ProductTariff tariff);
		Task<ProductTariff> GetByIdWithAccessCheck(string userId, int id);
		Task<List<ProductTariff>> GetListWithAccessCheckAsync(string userId, int productId);
	}
}