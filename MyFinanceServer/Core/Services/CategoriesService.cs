using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public class CategoriesService
    {
        private IRepository _repository;

        public CategoriesService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<Category> AddCategory(string userId, string title)
        {
            var user = await _repository.GetById<string, ApplicationUser>(userId);

            var categories = await _repository.List<Category>();

            var order = categories.Any() ? categories.Max(x => x.Order) + 1 : 0;

            var bank = await _repository.Add<Category>(new Category() { Title = title, Order = order, User = user });

            return bank;
        }

        public async Task DeleteCategory(string userId, int categoryId)
        {
            var user = await _repository.GetById<string, ApplicationUser>(userId);

            var category = await _repository.GetById<int, Category>(categoryId);

            category.IsBelongsTo(userId);

            await _repository.Delete<Category>(category);
        }
    }
}
