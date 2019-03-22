using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class TransactionTests
    {
        private readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();

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
            var user = new ApplicationUser() {Id = Guid.NewGuid().ToString(), Email = "Email #1"};
            dbContext.Users.Add(user);
            var bank = new Bank() {Title = "Bank #1", User = user};
            dbContext.Banks.Add(bank);
            var account = new BankAccount() {Transactions = new List<Transaction>(), Bank = bank};
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

            var controller = new TransactionsController(dbContext, _autoMapper.Create(dbContext));
            controller.AddControllerContext(user);
            var result = await controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }

        [Test]
        public async Task PatchTransactionCategory_NoContentResult()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("PatchTransaction_NoContentResult");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() {Email = "Email #1"};
            dbContext.Users.Add(user);
            var bank = new Bank() {Title = "Bank #1", User = user};
            dbContext.Banks.Add(bank);
            var account = new BankAccount() {Transactions = new List<Transaction>(), Bank = bank};
            dbContext.Accounts.Add(account);
            var transaction = new Transaction()
                {DateTime = DateTime.Now, Amount = 10, Description = "Description #1", Account = account};
            dbContext.Transactions.Add(transaction);
            var category = new Category() {Title = "Category #1", User = user};
            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();

            var controller = new TransactionsController(dbContext, _autoMapper.Create(dbContext));
            controller.AddControllerContext(user);
            var result = await controller.Patch(transaction.Id,
                new JsonPatchDocument<Api.Dto.Transaction>(new List<Operation<Api.Dto.Transaction>>()
                    {
                        new Operation<Api.Dto.Transaction>("replace", "/categoryId", null, category.Id)
                    },
                    new CamelCasePropertyNamesContractResolver()));

            Assert.IsInstanceOf<NoContentResult>(result);

            var newTransaction = await dbContext.Transactions.Include(x => x.Category)
                .SingleOrDefaultAsync(x => x.Id == transaction.Id);
            Assert.IsNotNull(newTransaction.Category);
            Assert.AreEqual(category.Id, newTransaction.Category.Id);
        }

        [Test]
        public async Task PatchTransactionNullCategory_NoContentResult()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("PatchTransaction_NoContentResult");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            var bank = new Bank() { Title = "Bank #1", User = user };
            dbContext.Banks.Add(bank);
            var account = new BankAccount() { Transactions = new List<Transaction>(), Bank = bank };
            dbContext.Accounts.Add(account);
            var category = new Category() { Title = "Category #1", User = user };
            dbContext.Categories.Add(category);
            var transaction = new Transaction()
            { DateTime = DateTime.Now, Amount = 10, Description = "Description #1", Account = account, Category = category };
            dbContext.Transactions.Add(transaction);
            await dbContext.SaveChangesAsync();

            var controller = new TransactionsController(dbContext, _autoMapper.Create(dbContext));
            controller.AddControllerContext(user);
            var result = await controller.Patch(transaction.Id,
                new JsonPatchDocument<Api.Dto.Transaction>(new List<Operation<Api.Dto.Transaction>>()
                    {
                        new Operation<Api.Dto.Transaction>("replace", "/categoryId", null)
                    }, 
                    new CamelCasePropertyNamesContractResolver()));

            Assert.IsInstanceOf<NoContentResult>(result);

            var newTransaction = await dbContext.Transactions.Include(x => x.Category)
                .SingleOrDefaultAsync(x => x.Id == transaction.Id);
            Assert.IsNull(newTransaction.Category);
        }
    }
}