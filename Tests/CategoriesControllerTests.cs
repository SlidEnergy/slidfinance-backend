using Microsoft.EntityFrameworkCore;
using Moq;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MyFinanceServer.Tests
{
    [TestFixture]
    public class CategoriesControllerTests
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

            var controller = new CategoriesController(_autoMapper.Create(_db), _service);
            controller.AddControllerContext(_user);
            var result = await controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }

        [Test]
        public async Task GetEmptyCategoryList_ShouldBeEmptyListReturned()
        {
            _repository.Setup(x => x.GetListWithAccessCheck(It.IsAny<string>())).ReturnsAsync(_user.Categories.ToList());

            var controller = new CategoriesController(_autoMapper.Create(_db), _service);
            controller.AddControllerContext(_user);
            var result = await controller.GetList();

            Assert.AreEqual(0, result.Value.Count());
        }
    }
}