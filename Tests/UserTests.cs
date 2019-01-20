using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Threading.Tasks;

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

            var controller = new UsersController(dbContext, _autoMapper.Create());
            controller.AddControllerContext(user);
            var result = await controller.GetCurrentUser();

            Assert.IsInstanceOf<ActionResult<Api.Dto.User>>(result);

            Assert.AreEqual(user.Id, result.Value.Id);
            Assert.AreEqual(user.Email, result.Value.Email);
        }
    }
}