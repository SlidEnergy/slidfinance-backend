using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class UserTests : TestBase
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetCurrentUser_Ok()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("GetCurrentUser_Ok");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new Models.User() { Id = 1, Password = "Password #1", Email = "Email #1" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var controller = new UsersController(dbContext);
            controller.ControllerContext = CreateControllerContext(user);
            var result = await controller.GetCurrentUser();

            Assert.IsInstanceOf<ActionResult<Models.User>>(result);

            Assert.AreEqual(user.Id, result.Value.Id);
            Assert.AreEqual(user.Email, result.Value.Email);
        }
    }
}