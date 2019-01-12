using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class TransactionsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task AddTransaction_Created()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("Add_Transaction_Created");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var account = new Models.Account() { Transactions = new List<Models.Transaction>() };
            await dbContext.Accounts.AddAsync(account);
            await dbContext.Banks.AddAsync(new Models.Bank()
            {
                Title = "Bank #1",
                Accounts = new List<Models.Account>(new[] { account })
            });
            await dbContext.SaveChangesAsync();

            var transaction = new TransactionBindingModel()
            {
                AccountId = account.Id,
                DateTime = DateTime.Now,
                Amount = 10,
                Category = "Category #1",
                Description = "Description #1"
            };

            var controller = new TransactionsController(dbContext);
            var result = await controller.AddTransaction(transaction);

            Assert.IsInstanceOf<ActionResult<Models.Transaction>>(result);
        }

        [Test]
        public async Task AddTransactions_NoContentResult()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("Add_Transaction_Created");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var account = new Models.Account() { Transactions = new List<Models.Transaction>() };
            await dbContext.Accounts.AddAsync(account);
            await dbContext.Banks.AddAsync(new Models.Bank()
            {
                Title = "Bank #1",
                Accounts = new List<Models.Account>(new[] { account })
            });
            await dbContext.SaveChangesAsync();

            var transaction1 = new TransactionBindingModel()
            {
                AccountId = account.Id,
                DateTime = DateTime.Now,
                Amount = 10,
                Category = "Category #1",
                Description = "Description #1"
            };

            var transaction2 = new TransactionBindingModel()
            {
                AccountId = account.Id,
                DateTime = DateTime.Now,
                Amount = 5,
                Category = "Category #2",
                Description = "Description #2"
            };

            var controller = new TransactionsController(dbContext);
            var result = await controller.AddTransactions(new[] { transaction1, transaction2 });

            Assert.IsInstanceOf<StatusCodeResult>(result);
            Assert.AreEqual(201, ((StatusCodeResult)result).StatusCode);
        }
    }
}