using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace MyFinanceServer.Tests
{
    public class UserTests
    {
        private readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();

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
            var user = new ApplicationUser() { UserName = "Email #1", Email = "Email #1" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            manager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            manager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            var controller = new UsersController(dbContext, _autoMapper.Create(dbContext), manager.Object);
            controller.AddControllerContext(user);
            var result = await controller.GetCurrentUser();

            Assert.IsInstanceOf<ActionResult<Api.Dto.User>>(result);

            Assert.AreEqual(user.Id, result.Value.Id);
            Assert.AreEqual(user.Email, result.Value.Email);
        }
    }
}