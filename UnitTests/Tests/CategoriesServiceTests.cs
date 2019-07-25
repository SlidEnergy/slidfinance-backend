using Microsoft.EntityFrameworkCore;
using Moq;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.UnitTests
{
    [TestFixture]
    public class CategoriesServiceTests : TestsBase
    {
        private CategoriesService _service;

        [SetUp]
        public void Setup()
        {
            _service = new CategoriesService(_mockedDal);
        }

        [Test]
        public async Task AddFirstCategory_ShouldBeCallAddMethodWithRightArguments()
        {
            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.Add(It.IsAny<Category>())).ReturnsAsync(new Category());
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

            var category1 = await _service.AddCategory(_user.Id, "Category #1");

            _categories.Verify(x => x.Add(
                It.Is<Category>(c => c.Title == "Category #1" && c.Order == 0 && c.User.Id == _user.Id)), Times.Exactly(1));
        }

        [Test]
        public async Task AddSecondCategory_ShouldBeCallAddMethodWithRightArguments()
        {
            await _dal.Categories.Add(new Category()
            {
                Title = "Category #1",
                User = _user
            });

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.Add(It.IsAny<Category>())).ReturnsAsync(new Category());
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

            var category2 = await _service.AddCategory(_user.Id, "Category #2");

            _categories.Verify(x => x.Add(
                It.Is<Category>(c => c.Title == "Category #2" && c.Order == 1 && c.User.Id == _user.Id)), Times.Exactly(1));
        }

        [Test]
        public async Task AddFirstCategory_ShouldBeReturnCategoryWithRightProperties()
        {
            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.Add(It.IsAny<Category>())).ReturnsAsync((Category x) => x);
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

            var category = await _service.AddCategory(_user.Id, "Category #1");

            Assert.AreEqual("Category #1", category.Title);
            Assert.AreEqual(0, category.Order);
            Assert.AreSame(_user, category.User);
        }

        [Test]
        public async Task AddSecondCategory_ShouldBeReturnCategoryWithRightProperties()
        {
            await _dal.Categories.Add(new Category()
            {
                Title = "Category #1",
                User = _user
            });

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.Add(It.IsAny<Category>())).ReturnsAsync((Category x) => x);
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

            var category = await _service.AddCategory(_user.Id, "Category #2");

            Assert.AreEqual("Category #2", category.Title);
            Assert.AreEqual(1, category.Order);
            Assert.AreSame(_user, category.User);
        }

        [Test]
        public async Task DeleteCategory_ShouldBeCallAddMethodWithRightArguments()
        {
            var category = await _dal.Categories.Add(new Category()
            {
                Title = "Category #1",
                User = _user
            });

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());
            _categories.Setup(x => x.Delete(It.IsAny<Category>())).Returns(Task.CompletedTask);
            _transactions.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(new List<Transaction>());

            await _service.DeleteCategory(_user.Id, category.Id);

            _categories.Verify(x => x.Delete(
                It.Is<Category>(c => c.Title == category.Title && c.Order == category.Order && category.User == _user)),
                Times.Exactly(1));
        }

        [Test]
        public async Task GetCategories_ShouldBeCallGetListMethodWithRightArguments()
        {
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

            var result = await _service.GetList(_user.Id);

            _categories.Verify(x => x.GetListWithAccessCheck(
                    It.Is<string>(c => c == _user.Id)),
                Times.Exactly(1));
        }

        [Test]
        public async Task GetCategories_ShouldBeListReturned()
        {
            await _dal.Categories.Add(new Category()
            {
                Title = "Category #1",
                User = _user
            });
            await _dal.Categories.Add(new Category()
            {
                Title = "Category #2",
                User = _user
            });

            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

            var result = await _service.GetList(_user.Id);

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetEmptyCategoryList_ShouldBeEmptyListReturned()
        {
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

            var result = await _service.GetList(_user.Id);

            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public async Task EditCategory_ShouldBeCallUpdateMethodWithRightArguments()
        {
            var category = await _dal.Categories.Add(new Category()
            {
                Title = "Category #1",
                User = _user
            });

            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());
            _categories.Setup(x => x.Update(It.IsAny<Category>())).ReturnsAsync((Category x) => x);

            await _service.EditCategory(_user.Id, category.Id, "Edited category", 4);

            _categories.Verify(x => x.Update(
                    It.Is<Category>(c => c.Title == "Edited category" && c.Order == 0 && category.User.Id == _user.Id)),
                Times.Exactly(1));
        }

        [Test]
        public async Task DeleteCategory_ShouldBeCategoriesReordered()
        {
            var category1 = await _dal.Categories.Add(new Category()
            {
                Title = "Category #1",
                Order = 0,
                User = _user
            });
            var category2 = await _dal.Categories.Add(new Category()
            {
                Title = "Category #2",
                Order = 1,
                User = _user
            });
            var category3 = await _dal.Categories.Add(new Category()
            {
                Title = "Category #3",
                Order = 2,
                User = _user
            });

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category2);
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.Where(x=>x.Id != category2.Id).ToList());
            _categories.Setup(x => x.Delete(It.IsAny<Category>())).Returns(Task.CompletedTask);
            _categories.Setup(x => x.Update(It.IsAny<Category>())).ReturnsAsync((Category category) => category);
            _transactions.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(new List<Transaction>());

            await _service.DeleteCategory(_user.Id, category2.Id);

            Assert.AreEqual(0, category1.Order);
            Assert.AreEqual(1, category3.Order);
            _categories.Verify(x => x.Update(It.Is<Category>(c => c.Id == category1.Id && c.Order == 0 && c.User == _user)), Times.Once);
            _categories.Verify(x => x.Update(It.Is<Category>(c => c.Id == category3.Id && c.Order == 1 && c.User == _user)), Times.Once);
        }

        [Test]
        public async Task DeleteCategory_TransactionsShouldBeMoved()
        {
            var category1 = await _dal.Categories.Add(new Category()
            {
                Title = "Category #1",
                Order = 0,
                User = _user
            });
            var category2 = await _dal.Categories.Add(new Category()
            {
                Title = "Category #2",
                Order = 1,
                User = _user
            });
            var transaction1 = await _dal.Transactions.Add(new Transaction()
            {
                Category = category1
            });
            var transaction2 = await _dal.Transactions.Add(new Transaction()
            {

            });

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.GetById(It.Is<int>(id => id == category1.Id))).ReturnsAsync(category1);
            _categories.Setup(x => x.GetById(It.Is<int>(id => id == category2.Id))).ReturnsAsync(category2);
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.Where(x => x.Id != category1.Id).ToList());
            _categories.Setup(x => x.Delete(It.IsAny<Category>())).Returns(Task.CompletedTask);
            _categories.Setup(x => x.Update(It.IsAny<Category>())).ReturnsAsync((Category category) => category);
            _transactions.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(new List<Transaction>() { transaction1, transaction2 });
            _transactions.Setup(x => x.Update(It.IsAny<Transaction>())).ReturnsAsync((Transaction transaction) => transaction);

            await _service.DeleteCategory(_user.Id, category1.Id, category2.Id);

            Assert.AreEqual(category2.Id, transaction1.Category.Id);
            Assert.AreEqual(null, transaction2.Category);
            _transactions.Verify(x => x.Update(It.Is<Transaction>(t => t.Id == transaction1.Id && t.Category.Id == category2.Id)), Times.Once);
        }

        [Test]
        public async Task DeleteCategory_TransactionsShouldBeMovedToEmptyCategory()
        {
            var category = await _dal.Categories.Add(new Category()
            {
                Title = "Category #1",
                Order = 0,
                User = _user
            });
            var transaction1 = await _dal.Transactions.Add(new Transaction()
            {
                Category = category
            });
            var transaction2 = await _dal.Transactions.Add(new Transaction()
            {

            });

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.Where(x => x.Id != category.Id).ToList());
            _categories.Setup(x => x.Delete(It.IsAny<Category>())).Returns(Task.CompletedTask);
            _categories.Setup(x => x.Update(It.IsAny<Category>())).ReturnsAsync((Category c) => c);
            _transactions.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(new List<Transaction>() { transaction1, transaction2 });
            _transactions.Setup(x => x.Update(It.IsAny<Transaction>())).ReturnsAsync((Transaction transaction) => transaction);

            await _service.DeleteCategory(_user.Id, category.Id, null);

            Assert.AreEqual(null, transaction1.Category);
            Assert.AreEqual(null, transaction2.Category);
            _transactions.Verify(x => x.Update(It.Is<Transaction>(t => t.Id == transaction1.Id && t.Category == null)), Times.Once);
        }

        [Test]
        public async Task EditCategory_ShouldBeCategoriesReordered()
        {
            var category1 = await _dal.Categories.Add(new Category()
            {
                Title = "Category #1",
                Order = 0,
                User = _user
            });
            var category2 = await _dal.Categories.Add(new Category()
            {
                Title = "Category #2",
                Order = 1,
                User = _user
            });
            var category3 = await _dal.Categories.Add(new Category()
            {
                Title = "Category #3",
                Order = 2,
                User = _user
            });

            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category3);
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());
            _categories.Setup(x => x.Update(It.IsAny<Category>())).ReturnsAsync((Category x) => x);

            var editedCategory = await _service.EditCategory(_user.Id, category3.Id, "Edited category", 1);

            Assert.AreEqual(0, category1.Order);
            Assert.AreEqual(1, category3.Order);
            Assert.AreEqual(2, category2.Order);
        }

        [Test]
        public async Task DeleteCategory_ShouldBeThrowEntityAccessDeniedException()
        {
            var user2 = await _dal.Users.Add(new ApplicationUser());

            var category = await _dal.Categories.Add(new Category()
            {
                Title = "Category #1",
                Order = 0,
                User = user2
            });

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.Where(x => x.Id != category.Id).ToList());
            _categories.Setup(x => x.Delete(It.IsAny<Category>())).Returns(Task.CompletedTask);

 
            Assert.ThrowsAsync<EntityAccessDeniedException>(() =>
                _service.DeleteCategory(_user.Id, category.Id));
        }

        [Test]
        public async Task EditCategory_ShouldBeThrowEntityAccessDeniedException()
        {
            var user2 = await _dal.Users.Add(new ApplicationUser());

            var category = await _dal.Categories.Add(new Category()
            {
                Title = "Category #1",
                Order = 0,
                User = user2
            });

            _categories.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _categories.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());
            _categories.Setup(x => x.Update(It.IsAny<Category>())).ReturnsAsync((Category x) => x);

            Assert.ThrowsAsync<EntityAccessDeniedException>(() =>
                _service.EditCategory(_user.Id, category.Id, "Edited category", 1));
        }
    }
}