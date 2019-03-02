using Microsoft.EntityFrameworkCore;
using Moq;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    [TestFixture]
    public class CategoriesServiceTests
    {
        private readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();
        private ApplicationDbContext _db;
        private DataAccessLayer _dal;
        private ApplicationUser _user;

        private Mock<ICategoriesRepository> _repository;
        private CategoriesService _service;

        [SetUp]
        public async Task Setup()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(TestContext.CurrentContext.Test.Name);
            _db = new ApplicationDbContext(optionsBuilder.Options);
            _dal = new DataAccessLayer(new EfCategoriesRepository(_db), new EfRepository(_db));
            _user = await _dal.Users.Add(new ApplicationUser() { Email = "Email #1" });

            _repository = new Mock<ICategoriesRepository>();
            _service = new CategoriesService(_repository.Object);
        }

        [Test]
        public async Task AddFirstCategory_ShouldBeCallAddMethodWithRightArguments()
        {
            _repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(_user);
            _repository.Setup(x => x.Add<Category>(It.IsAny<Category>())).ReturnsAsync(new Category());
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

            var category1 = await _service.AddCategory(_user.Id, "Category #1");

            _repository.Verify(x => x.Add<Category>(
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

            _repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(_user);
            _repository.Setup(x => x.Add<Category>(It.IsAny<Category>())).ReturnsAsync(new Category());
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

            var category2 = await _service.AddCategory(_user.Id, "Category #2");

            _repository.Verify(x => x.Add<Category>(
                It.Is<Category>(c => c.Title == "Category #2" && c.Order == 1 && c.User.Id == _user.Id)), Times.Exactly(1));
        }

        [Test]
        public async Task AddFirstCategory_ShouldBeReturnCategoryWithRightProperties()
        {
            _repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(_user);
            _repository.Setup(x => x.Add<Category>(It.IsAny<Category>())).ReturnsAsync((Category x) => x);
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

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

            _repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(_user);
            _repository.Setup(x => x.Add<Category>(It.IsAny<Category>())).ReturnsAsync((Category x) => x);
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

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

            _repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(_user);
            _repository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());
            _repository.Setup(x => x.Delete<Category>(It.IsAny<Category>())).Returns(Task.CompletedTask);

            await _service.DeleteCategory(_user.Id, category.Id);

            _repository.Verify(x => x.Delete(
                It.Is<Category>(c => c.Title == category.Title && c.Order == category.Order && category.User == _user)),
                Times.Exactly(1));
        }

        [Test]
        public async Task GetCategories_ShouldBeCallAddMethodWithRightArguments()
        {
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

            var result = await _service.GetList(_user.Id);

            _repository.Verify(x => x.GetListWithAccessCheck(
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

            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

            var result = await _service.GetList(_user.Id);

            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetEmptyCategoryList_ShouldBeEmptyListReturned()
        {
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

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

            _repository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());
            _repository.Setup(x => x.Update(It.IsAny<Category>())).ReturnsAsync((Category x) => x);

            await _service.EditCategory(_user.Id, category.Id, "Edited category", 4);

            _repository.Verify(x => x.Update(
                    It.Is<Category>(c => c.Title == "Edited category" && c.Order == 0 && category.User.Id == category.User.Id)),
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

            _repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(_user);
            _repository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category2);
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.Where(x=>x.Id != category2.Id).ToList());
            _repository.Setup(x => x.Delete<Category>(It.IsAny<Category>())).Returns(Task.CompletedTask);

            await _service.DeleteCategory(_user.Id, category2.Id);

            Assert.AreEqual(0, category1.Order);
            Assert.AreEqual(1, category3.Order);
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

            _repository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category3);
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());
            _repository.Setup(x => x.Update(It.IsAny<Category>())).ReturnsAsync((Category x) => x);

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

            _repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(_user);
            _repository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.Where(x => x.Id != category.Id).ToList());
            _repository.Setup(x => x.Delete<Category>(It.IsAny<Category>())).Returns(Task.CompletedTask);

 
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

            _repository.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(category);
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());
            _repository.Setup(x => x.Update(It.IsAny<Category>())).ReturnsAsync((Category x) => x);

            Assert.ThrowsAsync<EntityAccessDeniedException>(() =>
                _service.EditCategory(_user.Id, category.Id, "Edited category", 1));
        }
    }
}