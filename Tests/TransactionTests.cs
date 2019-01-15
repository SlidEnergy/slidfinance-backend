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
using MyFinanceServer.Models;

namespace MyFinanceServer.Tests
{
    public class TransactionTests
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
            await dbContext.Transactions.AddAsync(new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 10,
                Category = Category.None,
                Description = "Description #1"
            });
            await dbContext.Transactions.AddAsync(new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 5,
                Category = Category.None,
                Description = "Description #2"
            });
            await dbContext.SaveChangesAsync();

            var controller = new TransactionsController(dbContext);
            var result = await controller.GetTransactions();

            Assert.IsInstanceOf<ActionResult<IEnumerable<Models.Transaction>>>(result);

            Assert.AreEqual(2, ((ActionResult<IEnumerable<Models.Transaction>>)result).Value.Count());
        }
    }
}