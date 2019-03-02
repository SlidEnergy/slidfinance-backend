using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public class CategoriesService
    {
        private ICategoriesRepository _repository;

        public CategoriesService(ICategoriesRepository repository)
        {
            _repository = repository;
        }


        public async Task<List<Category>> GetList(string userId)
        {
            var categories = await _repository.GetListWithAccessCheck(userId);

            return categories.OrderBy(x => x.Order).ToList();
        }

        public async Task<Category> AddCategory(string userId, string title)
        {
            var user = await _repository.GetById<string, ApplicationUser>(userId);

            var categories = await _repository.GetListWithAccessCheck(userId);

            var order = categories.Any() ? categories.Max(x => x.Order) + 1 : 0;

            var category = await _repository.Add<Category>(new Category() { Title = title, Order = order, User = user });

            return category;
        }

        public async Task DeleteCategory(string userId, int categoryId)
        {
            var user = await _repository.GetById<string, ApplicationUser>(userId);

            var category = await _repository.GetById(categoryId);

            if(!category.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

            await _repository.Delete<Category>(category);

            await ReorderAllCategories(userId);
        }

        public async Task<Category> EditCategory(string userId, int categoryId, string title, int order)
        {
            var editCategory = await _repository.GetById(categoryId);

            if (!editCategory.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

            editCategory.Rename(title);

            var categories = await _repository.GetListWithAccessCheck(userId);

            // Если порядок категории вышел за пределы, устанавливаем его последним
            if(order > categories.Count - 1)
                order = categories.Count - 1;

            if (editCategory.Order != order)
            {
                editCategory.SetOrder(order);
                await ReorderCategories(userId, editCategory, order);
            }

            await _repository.Update(editCategory);

            return editCategory;
        }

        private async Task ReorderAllCategories(string userId)
        {
            var categories = await _repository.GetListWithAccessCheck(userId);

            int newOrder = 0;

            foreach (Category c in categories.OrderBy(x => x.Order).ToList())
            {
                c.SetOrder(newOrder);
                newOrder++;
            }
        }

        private async Task ReorderCategories(string userId, Category category, int order)
        {
            var categories = await _repository.GetListWithAccessCheck(userId);

            int newOrder = order + 1;

            foreach (Category c in categories.Where(x => x.Order >= order && x.Id != category.Id).OrderBy(x => x.Order).ToList())
            {
                c.SetOrder(newOrder);
                newOrder++;
            }
        }
    }
}
