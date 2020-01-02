using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	[TestFixture]
    public class CategoriesServiceTests : TestsBase
    {
        private CategoriesService _service;

        [SetUp]
        public void Setup()
        {
            _service = new CategoriesService(_mockedDal, _db);
        }

        [Test]
        public async Task AddFirstCategory_ShouldBeCallAddMethodWithRightArguments()
        {
            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.Add(It.IsAny<UserCategory>())).ReturnsAsync(new UserCategory());

            var category1 = await _service.AddCategory(_user.Id, "Category #1");

            _categories.Verify(x => x.Add(
                It.Is<UserCategory>(c => c.Title == "Category #1" && c.Order == 0)), Times.Exactly(1));
        }

        [Test]
        public async Task AddSecondCategory_ShouldBeCallAddMethodWithRightArguments()
        {
			var category = await _db.CreateCategory(_user);

            _categories.Setup(x => x.Add(It.IsAny<UserCategory>())).ReturnsAsync(new UserCategory());

            var category2 = await _service.AddCategory(_user.Id, "Category #2");

            _categories.Verify(x => x.Add(
                It.Is<UserCategory>(c => c.Title == "Category #2" && c.Order == 1)), Times.Exactly(1));
        }

        [Test]
        public async Task AddFirstCategory_ShouldBeReturnCategoryWithRightProperties()
        {
            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.Add(It.IsAny<UserCategory>())).ReturnsAsync((UserCategory x) => x);

            var category = await _service.AddCategory(_user.Id, "Category #1");

            Assert.AreEqual("Category #1", category.Title);
            Assert.AreEqual(0, category.Order);
        }

        [Test]
        public async Task AddSecondCategory_ShouldBeReturnCategoryWithRightProperties()
        {
			await _db.CreateCategory(_user);

            _categories.Setup(x => x.Add(It.IsAny<UserCategory>())).ReturnsAsync((UserCategory x) => x);

            var category = await _service.AddCategory(_user.Id, "Category #2");

            Assert.AreEqual("Category #2", category.Title);
            Assert.AreEqual(1, category.Order);
        }

        [Test]
        public async Task GetCategories_ShouldBeListReturned()
        {
			var category1 = await _db.CreateCategory(_user);
			var category2 = await _db.CreateCategory(_user);

            var result = await _service.GetListWithAccessCheckAsync(_user.Id);

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetEmptyCategoryList_ShouldBeEmptyListReturned()
        {
            var result = await _service.GetListWithAccessCheckAsync(_user.Id);

            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public async Task EditCategory_ShouldBeCallUpdateMethodWithRightArguments()
        {
			var category = await _db.CreateCategory(_user);

            await _service.EditCategory(_user.Id, category.Id, "Edited category", 4);
        }

        [Test]
        public async Task DeleteCategory_ShouldBeCategoriesReordered()
        {
			var category1 = await _db.CreateCategory(_user);
			var category2 = await _db.CreateCategory(_user);
			var category3 = await _db.CreateCategory(_user);
			category1.Order = 0;
			category2.Order = 1;
			category3.Order = 2;
			await _db.SaveChangesAsync();

            await _service.DeleteCategory(_user.Id, category2.Id);

            Assert.AreEqual(0, category1.Order);
            Assert.AreEqual(1, category3.Order);
        }

        [Test]
        public async Task DeleteCategory_TransactionsShouldBeMoved()
        {
			var account = await _db.CreateAccount(_user);
			var category1 = await _db.CreateCategory(_user);
			var category2 = await _db.CreateCategory(_user);
			category1.Order = 0;
			category2.Order = 1;
			await _db.SaveChangesAsync();
			var transaction1 = await _db.CreateTransaction(account);
			transaction1.Category = category1;
			await _db.SaveChangesAsync();
			var transaction2 = await _db.CreateTransaction(account);

            _categories.Setup(x => x.Delete(It.IsAny<UserCategory>())).Returns(Task.CompletedTask);
            _categories.Setup(x => x.Update(It.IsAny<UserCategory>())).ReturnsAsync((UserCategory category) => category);
            _transactions.Setup(x => x.Update(It.IsAny<Transaction>())).ReturnsAsync((Transaction transaction) => transaction);

            await _service.DeleteCategory(_user.Id, category1.Id, category2.Id);

            Assert.AreEqual(category2.Id, transaction1.Category.Id);
            Assert.AreEqual(null, transaction2.Category);
            _transactions.Verify(x => x.Update(It.Is<Transaction>(t => t.Id == transaction1.Id && t.Category.Id == category2.Id)), Times.Once);
        }

        [Test]
        public async Task DeleteCategory_TransactionsShouldBeMovedToEmptyCategory()
        {
			var category = await _db.CreateCategory(_user);
			var account = await _db.CreateAccount(_user);
			var transaction1 = await _db.CreateTransaction(account);
			transaction1.Category = category;
			await _db.SaveChangesAsync();
            var transaction2 = await _db.CreateTransaction(account);

            _categories.Setup(x => x.Delete(It.IsAny<UserCategory>())).Returns(Task.CompletedTask);
            _categories.Setup(x => x.Update(It.IsAny<UserCategory>())).ReturnsAsync((UserCategory c) => c);
            _transactions.Setup(x => x.Update(It.IsAny<Transaction>())).ReturnsAsync((Transaction transaction) => transaction);

            await _service.DeleteCategory(_user.Id, category.Id, null);

            Assert.AreEqual(null, transaction1.Category);
            Assert.AreEqual(null, transaction2.Category);
            _transactions.Verify(x => x.Update(It.Is<Transaction>(t => t.Id == transaction1.Id && t.Category == null)), Times.Once);
        }

        [Test]
        public async Task EditCategory_ShouldBeCategoriesReordered()
        {
			var category1 = await _db.CreateCategory(_user);
			var category2 = await _db.CreateCategory(_user);
			var category3 = await _db.CreateCategory(_user);
			category1.Order = 0;
			category2.Order = 1;
			category3.Order = 2;
			await _db.SaveChangesAsync();

            _categories.Setup(x => x.Update(It.IsAny<UserCategory>())).ReturnsAsync((UserCategory x) => x);

            var editedCategory = await _service.EditCategory(_user.Id, category3.Id, "Edited category", 1);

            Assert.AreEqual(0, category1.Order);
            Assert.AreEqual(1, category3.Order);
            Assert.AreEqual(2, category2.Order);
        }

        [Test]
        public async Task DeleteCategory_ShouldBeThrowEntityAccessDeniedException()
        {
			var user2 = await _db.CreateUser();

			var category = await _db.CreateCategory(user2);

			_categories.Setup(x => x.Delete(It.IsAny<UserCategory>())).Returns(Task.CompletedTask);
 
            Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _service.DeleteCategory(_user.Id, category.Id));
        }

        [Test]
        public async Task EditCategory_ShouldBeThrowEntityAccessDeniedException()
        {
			var user2 = await _db.CreateUser();

			var category = await _db.CreateCategory(user2);

            _categories.Setup(x => x.Update(It.IsAny<UserCategory>())).ReturnsAsync((UserCategory x) => x);

            Assert.ThrowsAsync<EntityNotFoundException>(() =>
                _service.EditCategory(_user.Id, category.Id, "Edited category", 1));
        }
    }
}