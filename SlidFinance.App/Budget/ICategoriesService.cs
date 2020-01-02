using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface ICategoriesService
	{
		Task<UserCategory> AddCategory(string userId, string title);
		Task DeleteCategory(string userId, int categoryId, int? moveCategoryId = null);
		Task<UserCategory> EditCategory(string userId, int categoryId, string title, int order);
		Task<List<UserCategory>> GetListWithAccessCheckAsync(string userId);
	}
}