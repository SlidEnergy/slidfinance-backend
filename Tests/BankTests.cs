using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using MyFinanceServer.Domain;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using MyFinanceServer.Controllers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MyFinanceServer.Tests
{
    public class BankTests : TestBase
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetBanks_Ok()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("Get_Banks");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new Models.User() { Id = 1, Password = "Password #1", Email = "Email #1" };
            await dbContext.Users.AddAsync(user);
            await dbContext.Banks.AddAsync(new Models.Bank()
            {
                Title = "Bank #1",
                User = user
            });
            await dbContext.Banks.AddAsync(new Models.Bank()
            {
                Title = "Bank #2",
                User = user
            });
            await dbContext.SaveChangesAsync();

            var controller = new BanksController(dbContext);
            controller.ControllerContext = CreateControllerContext(user);
            var result = await controller.GetBanks();

            Assert.IsInstanceOf<ActionResult<IEnumerable<Models.Bank>>>(result);

            Assert.AreEqual(2, ((ActionResult<IEnumerable<Models.Bank>>)result).Value.Count());
        }
    }
}