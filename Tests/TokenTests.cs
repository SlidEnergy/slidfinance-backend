using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class TokenTests : TestBase
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
            var user = new Models.User() { Email = "email@domain.com", Password = "Password #1" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var tokenGenerator = new TokenGenerator(Options.Create(new AppSettings() { Secret = "Very very very long secret #1" }));
            var userBindingModel = new UserBindingModel() { Email = user.Email, Password = user.Password };

            var controller = new TokensController(tokenGenerator, dbContext);
            var result = controller.CreateToken(userBindingModel);

            Assert.IsInstanceOf<ActionResult<TokenInfo>>(result);
            var actionResult = (ActionResult<TokenInfo>)result;

            Assert.NotNull(actionResult.Value.Token);
            Assert.IsNotEmpty(actionResult.Value.Token);
            Assert.AreEqual(user.Email, actionResult.Value.Email);
        }
    }
}