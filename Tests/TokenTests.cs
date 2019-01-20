using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;

namespace MyFinanceServer.Tests
{
    public class TokenTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetToken_Ok()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("Add_Transaction_Created");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "email@domain.com" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var tokenGenerator = new TokenGenerator(Options.Create(new AppSettings() { Secret = "Very very very long secret #1" }));
            var userBindingModel = new UserBindingModel() { Email = user.Email, Password = "Password #1"};

            var store = new Mock<IUserStore<ApplicationUser>>();
            var manager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);
            manager.Setup(x => x.CheckPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            manager.Setup(x => x.FindByNameAsync(It.IsAny<string>())).Returns(Task.FromResult(user));

            var controller = new TokensController(tokenGenerator, dbContext, manager.Object);
            var result = await controller.CreateToken(userBindingModel);

            Assert.NotNull(result.Value.Token);
            Assert.IsNotEmpty(result.Value.Token);
            Assert.AreEqual(user.Email, result.Value.Email);
        }
    }
}