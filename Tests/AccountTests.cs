using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using MyFinanceServer.Domain;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;

namespace MyFinanceServer.Tests
{
    public class AccountTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task PatchAccountDataInMemory_NoContentResult()
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
                DateTime = DateTime.Now,
                Amount = 10,
                Category = "Category #1",
                Description = "Description #1"
            };

            var transaction2 = new TransactionBindingModel()
            {
                DateTime = DateTime.Now,
                Amount = 5,
                Category = "Category #2",
                Description = "Description #2"
            };

            var accountDataSaver = new AccountDataSaver(dbContext);
            var controller = new AccountsController(dbContext, accountDataSaver);
            var result = await controller.PatchAccountData(account.Id,
                new PatchAccountDataBindingModel { Balance = 500, Transactions = new[] { transaction1, transaction2 } });

            Assert.IsInstanceOf<NoContentResult>(result);

            var newAccount = await dbContext.Accounts.Include(x => x.Transactions).SingleOrDefaultAsync(x => x.Id == account.Id);
            Assert.AreEqual(500, newAccount.Balance);
            Assert.AreEqual(2, newAccount.Transactions.Count);
        }

        [Test]
        public async Task PatchAccountData_NoContentResult()
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
                DateTime = DateTime.Now,
                Amount = 10,
                Category = "Category #1",
                Description = "Description #1"
            };

            var transaction2 = new TransactionBindingModel()
            {
                DateTime = DateTime.Now,
                Amount = 5,
                Category = "Category #2",
                Description = "Description #2"
            };

            var accountDataSaverMock = new Mock<IAccountDataSaver>();
            accountDataSaverMock
                .Setup(x => 
                    x.Save(It.IsAny<Models.Account>(), It.IsAny<float>(), It.IsAny<ICollection<Models.Transaction>>()))
                .Returns(Task.CompletedTask);

            var controller = new AccountsController(dbContext, accountDataSaverMock.Object);
            var result = await controller.PatchAccountData(account.Id,
                new PatchAccountDataBindingModel { Balance = 500, Transactions = new[] { transaction1, transaction2 } });

            Assert.IsInstanceOf<NoContentResult>(result);

            accountDataSaverMock.Verify(x=> 
                x.Save(It.IsAny<Models.Account>(), It.IsAny<float>(), It.IsAny<ICollection<Models.Transaction>>()),
                Times.Once);
        }
    }
}