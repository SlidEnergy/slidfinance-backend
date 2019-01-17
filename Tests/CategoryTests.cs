using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class CategoryTests : TestBase
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetCategories_Ok()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("GetCategories_Ok");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new Models.User() { Id = 1, Password = "Password #1", Email = "Email #1" };
            await dbContext.Users.AddAsync(user);
            await dbContext.Category.AddAsync(new Models.Category()
            {
                Title = "Category #1",
                User = user
            });
            await dbContext.Category.AddAsync(new Models.Category()
            {
                Title = "Category #2",
                User = user
            });
            await dbContext.SaveChangesAsync();

            var controller = new CategoriesController(dbContext);
            controller.ControllerContext = CreateControllerContext(user);
            var result = await controller.GetCategory();

            Assert.IsInstanceOf<ActionResult<IEnumerable<Models.Category>>>(result);

            Assert.AreEqual(2, result.Value.Count());
        }
    }
}