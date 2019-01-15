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

namespace MyFinanceServer.Tests
{
    public class BankTests
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

            await dbContext.Banks.AddAsync(new Models.Bank()
            {
                Title = "Bank #1",
            });
            await dbContext.Banks.AddAsync(new Models.Bank()
            {
                Title = "Bank #2",
            });
            await dbContext.SaveChangesAsync();

            var controller = new BanksController(dbContext);
            var result = await controller.GetBanks();

            Assert.IsInstanceOf<ActionResult<IEnumerable<Models.Bank>>>(result);

            Assert.AreEqual(2, ((ActionResult<IEnumerable<Models.Bank>>)result).Value.Count());
        }
    }
}