using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface ICategoriesService
	{
		Task<Category> AddCategory(string userId, string title);
		Task DeleteCategory(string userId, int categoryId, int? moveCategoryId = null);
		Task<Category> EditCategory(string userId, int categoryId, string title, int order);
		Task<List<Category>> GetListWithAccessCheckAsync(string userId);
	}
}