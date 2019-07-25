using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
    public class CategoriesService
    {
        private DataAccessLayer _dal;

        public CategoriesService(DataAccessLayer dal)
        {
            _dal = dal;
        }

        public async Task<List<Category>> GetList(string userId)
        {
            var categories = await _dal.Categories.GetListWithAccessCheck(userId);

            return categories.OrderBy(x => x.Order).ToList();
        }

        public async Task<Category> AddCategory(string userId, string title)
        {
            var user = await _dal.Users.GetById(userId);

            var categories = await _dal.Categories.GetListWithAccessCheck(userId);

            var order = categories.Any() ? categories.Max(x => x.Order) + 1 : 0;

            var category = await _dal.Categories.Add(new Category() { Title = title, Order = order, User = user });

            return category;
        }

        public async Task DeleteCategory(string userId, int categoryId, int? moveCategoryId = null)
        {
            var user = await _dal.Users.GetById(userId);

            var category = await _dal.Categories.GetById(categoryId);

            if(!category.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

            // Перемещаем все транзакции в новую категорию, если она указана, или очищаем категорию

            var allTransactions = await _dal.Transactions.GetListWithAccessCheck(userId);

            var moveCategory = moveCategoryId == null ? null : await _dal.Categories.GetById(moveCategoryId.Value);

            var categoryTransactions = allTransactions.Where(x => x.Category != null && x.Category.Id == categoryId).ToList();

            foreach (var transaction in categoryTransactions) {
                transaction.Category = moveCategory;

                await _dal.Transactions.Update(transaction);
            }

            // Удаляем категорию

            await _dal.Categories.Delete(category);

            await ReorderAllCategories(userId);
        }

        public async Task<Category> EditCategory(string userId, int categoryId, string title, int order)
        {
            var editCategory = await _dal.Categories.GetById(categoryId);

            if (!editCategory.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

            editCategory.Rename(title);

            var categories = await _dal.Categories.GetListWithAccessCheck(userId);

            // Если порядок категории вышел за пределы, устанавливаем его последним
            if(order > categories.Count - 1)
                order = categories.Count - 1;

            if (editCategory.Order != order)
            {
                editCategory.SetOrder(order);
                await ReorderCategories(userId, editCategory, order);
            }

            await _dal.Categories.Update(editCategory);

            return editCategory;
        }

        private async Task ReorderAllCategories(string userId)
        {
            var categories = await _dal.Categories.GetListWithAccessCheck(userId);

            int newOrder = 0;

            foreach (Category c in categories.OrderBy(x => x.Order).ToList())
            {
                c.SetOrder(newOrder);
                await _dal.Categories.Update(c);
                newOrder++;
            }
        }

        private async Task ReorderCategories(string userId, Category category, int order)
        {
            var categories = await _dal.Categories.GetListWithAccessCheck(userId);

            int newOrder = order + 1;

            foreach (Category c in categories.Where(x => x.Order >= order && x.Id != category.Id).OrderBy(x => x.Order).ToList())
            {
                c.SetOrder(newOrder);
                await _dal.Categories.Update(c);
                newOrder++;
            }
        }
    }
}
