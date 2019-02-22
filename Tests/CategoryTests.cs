using Microsoft.EntityFrameworkCore;
using Moq;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class CategoryTests
    {
        private readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task AddFirstCategory_ShouldBeCallAddMethodWithRightArguments()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("AddFirstCategory_ShouldBeCallAddMethodWithRightArguments");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var repository = new Mock<IRepository>();
            repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(user);
            repository.Setup(x => x.Add<Category>(It.IsAny<Category>())).ReturnsAsync(new Category());
            repository.Setup(x => x.List<Category>()).ReturnsAsync(user.Categories.ToList());

            var categoriesService = new CategoriesService(repository.Object);

            var category1 = categoriesService.AddCategory(user.Id, "Category #1");

            repository.Verify(x => x.Add<Category>(
                It.Is<Category>(c => c.Title == "Category #1" && c.Order == 0 && c.User.Id == user.Id)), Times.Exactly(1));
        }

        [Test]
        public async Task AddSecondCategory_ShouldBeCallAddMethodWithRightArguments()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("AddSecondCategory_ShouldBeCallAddMethodWithRightArguments");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            dbContext.Categories.Add(new Category()
            {
                Title = "Category #1",
                User = user
            });
            await dbContext.SaveChangesAsync();

            var repository = new Mock<IRepository>();
            repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(user);
            repository.Setup(x => x.Add<Category>(It.IsAny<Category>())).ReturnsAsync(new Category());
            repository.Setup(x => x.List<Category>()).ReturnsAsync(user.Categories.ToList());

            var categoriesService = new CategoriesService(repository.Object);

            var category2 = categoriesService.AddCategory(user.Id, "Category #2");

            repository.Verify(x => x.Add<Category>(
                It.Is<Category>(c => c.Title == "Category #2" && c.Order == 1 && c.User.Id == user.Id)), Times.Exactly(1));
        }

        [Test]
        public async Task DeleteCategory_ShouldBeCallAddMethodWithRightArguments()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("DeleteCategory_ShouldBeCallAddMethodWithRightArguments");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            var category = new Category()
            {
                Title = "Category #1",
                User = user
            };
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var repository = new Mock<IRepository>();
            repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(user);
            repository.Setup(x => x.GetById<int, Category>(It.IsAny<int>())).ReturnsAsync(category);
            repository.Setup(x => x.Delete<Category>(It.IsAny<Category>())).Returns(Task.CompletedTask);

            var categoriesService = new CategoriesService(repository.Object);

            await categoriesService.DeleteCategory(user.Id, category.Id);

            repository.Verify(x => x.Delete<Category>(
                It.Is<Category>(c => c.Title == category.Title && c.Order == category.Order && category.User.Id == category.User.Id)),
                Times.Exactly(1));
        }

        [Test]
        public async Task GetCategories_Ok()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("GetCategories_Ok");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            dbContext.Categories.Add(new Category()
            {
                Title = "Category #1",
                User = user
            });
            dbContext.Categories.Add(new Category()
            {
                Title = "Category #2",
                User = user
            });
            await dbContext.SaveChangesAsync();

            var repository = new Mock<IRepository>();
            repository.Setup(x => x.Add<Category>(It.IsAny<Category>())).ReturnsAsync(new Category());

            var controller = new CategoriesController(dbContext, _autoMapper.Create(dbContext), new CategoriesService(repository.Object));
            controller.AddControllerContext(user);
            var result = await controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }
    }
}