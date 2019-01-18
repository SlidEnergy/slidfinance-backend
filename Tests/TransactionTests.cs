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
using MyFinanceServer.Domain;

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
            var user = new Models.User() {Id = 1, Password = "Password #1", Email = "Email #1"};
            dbContext.Users.Add(user);
            var bank = new Models.Bank() {Title = "Bank #1", User = user};
            dbContext.Banks.Add(bank);
            var account = new Models.Account() {Transactions = new List<Models.Transaction>(), Bank = bank};
            dbContext.Accounts.Add(account);
            dbContext.Transactions.Add(new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 10,
                Description = "Description #1",
                Account = account
            });
            dbContext.Transactions.Add(new Transaction()
            {
                DateTime = DateTime.Now,
                Amount = 5,
                Description = "Description #2",
                Account = account
            });
            await dbContext.SaveChangesAsync();

            var controller = new TransactionsController(dbContext);
            controller.ControllerContext = CreateControllerContext(user);
            var result = await controller.GetTransactions();

            Assert.IsInstanceOf<ActionResult<IEnumerable<Models.Transaction>>>(result);

            Assert.AreEqual(2, ((ActionResult<IEnumerable<Models.Transaction>>) result).Value.Count());
        }

        [Test]
        public async Task PatchTransaction_NoContentResult()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("PatchTransaction_NoContentResult");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new Models.User() {Id = 1, Password = "Password #1", Email = "Email #1"};
            dbContext.Users.Add(user);
            var bank = new Models.Bank() {Title = "Bank #1", User = user};
            dbContext.Banks.Add(bank);
            var account = new Models.Account() {Transactions = new List<Models.Transaction>(), Bank = bank};
            dbContext.Accounts.Add(account);
            var transaction = new Transaction()
                {DateTime = DateTime.Now, Amount = 10, Description = "Description #1", Account = account};
            dbContext.Transactions.Add(transaction);
            var category = new Models.Category() {Title = "Category #1", User = user};
            dbContext.Category.Add(category);
            await dbContext.SaveChangesAsync();

            var controller = new TransactionsController(dbContext);
            controller.ControllerContext = CreateControllerContext(user);
            var result = await controller.PatchTransaction(transaction.Id,
                new PatchTransactionBindingModel() {CategoryId = category.Id});

            Assert.IsInstanceOf<NoContentResult>(result);

            var newTransaction = await dbContext.Transactions.Include(x => x.Category)
                .SingleOrDefaultAsync(x => x.Id == transaction.Id);
            Assert.IsNotNull(newTransaction.Category);
            Assert.AreEqual(category.Id, newTransaction.Category.Id);
        }
    }
}