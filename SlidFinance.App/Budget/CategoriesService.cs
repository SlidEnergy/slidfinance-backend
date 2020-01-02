using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
    public class CategoriesService : ICategoriesService
	{
        private DataAccessLayer _dal;
		private IApplicationDbContext _context;

		public CategoriesService(DataAccessLayer dal, IApplicationDbContext context)
        {
            _dal = dal;
			_context = context;
		}

        public async Task<List<UserCategory>> GetListWithAccessCheckAsync(string userId)
        {
			var categories = await _context.GetCategoryListWithAccessCheckAsync(userId);

			return categories.OrderBy(x => x.Order).ToList();
        }

        public async Task<UserCategory> AddCategory(string userId, string title)
        {
            var user = await _context.Users.FindAsync(userId);

            var categories = await _context.GetCategoryListWithAccessCheckAsync(userId);

            var order = categories.Any() ? categories.Max(x => x.Order) + 1 : 0;

            var category = await _dal.Categories.Add(new UserCategory() { Title = title, Order = order });
			_context.TrusteeCategories.Add(new TrusteeCategory(user, category));
			await _context.SaveChangesAsync();

            return category;
        }

        public async Task DeleteCategory(string userId, int categoryId, int? moveCategoryId = null)
        {
            var category = await _context.GetCategorByIdWithAccessCheckAsync(userId, categoryId);

            if(category == null)
                throw new EntityNotFoundException();

            // Перемещаем все транзакции в новую категорию, если она указана, или очищаем категорию

            var allTransactions = await _context.GetTransactionListWithAccessCheckAsync(userId);

            var moveCategory = moveCategoryId == null ? null : await _context.GetCategorByIdWithAccessCheckAsync(userId, moveCategoryId.Value);

            var categoryTransactions = allTransactions.Where(x => x.Category != null && x.Category.Id == categoryId).ToList();

            foreach (var transaction in categoryTransactions) {
                transaction.Category = moveCategory;

                await _dal.Transactions.Update(transaction);
            }

			// Удаляем категорию

			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();

            await ReorderAllCategories(userId);
        }

        public async Task<UserCategory> EditCategory(string userId, int categoryId, string title, int order)
        {
            var editCategory = await _context.GetCategorByIdWithAccessCheckAsync(userId, categoryId);

            if (editCategory == null)
                throw new EntityNotFoundException();

            editCategory.Rename(title);

            var categories = await _context.GetCategoryListWithAccessCheckAsync(userId);

            // Если порядок категории вышел за пределы, устанавливаем его последним
            if(order > categories.Count - 1)
                order = categories.Count - 1;

            if (editCategory.Order != order)
            {
                editCategory.SetOrder(order);
                await ReorderCategories(userId, editCategory, order);
            }

			_context.Categories.Update(editCategory);
			await _context.SaveChangesAsync();

            return editCategory;
        }

        private async Task ReorderAllCategories(string userId)
        {
            var categories = await _context.GetCategoryListWithAccessCheckAsync(userId);

            int newOrder = 0;

            foreach (UserCategory c in categories.OrderBy(x => x.Order).ToList())
            {
                c.SetOrder(newOrder);
                _context.Categories.Update(c);
				await _context.SaveChangesAsync();
                newOrder++;
            }
        }

        private async Task ReorderCategories(string userId, UserCategory category, int order)
        {
            var categories = await _context.GetCategoryListWithAccessCheckAsync(userId);

            int newOrder = order + 1;

            foreach (UserCategory c in categories.Where(x => x.Order >= order && x.Id != category.Id).OrderBy(x => x.Order).ToList())
            {
                c.SetOrder(newOrder);
				_context.Categories.Update(c);
				await _context.SaveChangesAsync();
				newOrder++;
            }
        }

		public async Task<UserCategory> GetByIdWithChecks(string userId, int id)
		{
			var category = await _dal.Categories.GetById(id);

			if (category == null)
				throw new EntityNotFoundException();

			await CheckAccessAndThrowException(userId, category);

			return category;
		}

		private async Task CheckAccessAndThrowException(string userId, UserCategory category)
		{
			var user = await _context.Users.FindAsync(userId);

			await CheckAccessAndThrowException(user, category);
		}

		private async Task CheckAccessAndThrowException(ApplicationUser user, UserCategory category)
		{
			var trustee = await _context.TrusteeCategories
				.Where(t => t.TrusteeId == user.TrusteeId)
				.Join(_context.Categories, t => t.CategoryId, c => c.Id, (t, c) => c)
				.Where(c => c.Id == category.Id)
				.FirstOrDefaultAsync();

			if (trustee == null)
				throw new EntityAccessDeniedException();
		}
	}
}
