using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using MyFinanceServer.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class TransactionTests : TestBase
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetTransactions_Ok()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("GetTransactions_Ok");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new Models.User() { Id = 1, Password = "Password #1", Email = "Email #1" };
            await dbContext.Users.AddAsync(user);
            var bank = new Models.Bank() { Title = "Bank #1", User = user };
            await dbContext.Banks.AddAsync(bank);
            var account = new Models.Account() { Transactions = new List<Models.Transaction>(), Bank = bank };
            await dbContext.Accounts.AddAsync(account);
            await dbContext.Transactions.AddAsync(new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 10,
                Category = Category.None,
                Description = "Description #1",
                Account = account
            });
            await dbContext.Transactions.AddAsync(new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 5,
                Category = Category.None,
                Description = "Description #2",
                Account = account
            });
            await dbContext.SaveChangesAsync();

            var controller = new TransactionsController(dbContext);
            controller.ControllerContext = CreateControllerContext(user);
            var result = await controller.GetTransactions();

            Assert.IsInstanceOf<ActionResult<IEnumerable<Models.Transaction>>>(result);

            Assert.AreEqual(2, ((ActionResult<IEnumerable<Models.Transaction>>)result).Value.Count());
        }
    }
}