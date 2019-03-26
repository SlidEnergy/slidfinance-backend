using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using MyFinanceServer.Core;

namespace MyFinanceServer.Tests
{
    public class AccountTests
    {
        private readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetAccounts_Ok()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("GetAccounts_Ok");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            var bank = new Bank() { Title = "Bank #1", User = user };
            dbContext.Banks.Add(bank);
            dbContext.Accounts.Add(new BankAccount() { Title = "Account #1", Bank = bank });
            dbContext.Accounts.Add(new BankAccount() { Title = "Account #2", Bank = bank });

            await dbContext.SaveChangesAsync();

            var accountDataSaver = new AccountDataSaver(dbContext);
            var controller = new AccountsController(dbContext, accountDataSaver, _autoMapper.Create(dbContext));
            controller.AddControllerContext(user);
            var result = await controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }


        [Test]
        public async Task PatchAccountData_NoContentResult()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("PatchAccountData_NoContentResult");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            var bank = new Bank() { Title = "Bank #1", User = user };
            dbContext.Banks.Add(bank);
            var account = new BankAccount() { Code ="code_1", Transactions = new List<Transaction>(), Bank = bank };
            dbContext.Accounts.Add(account);
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
            var controller = new AccountsController(dbContext, accountDataSaver, _autoMapper.Create(dbContext));
            controller.AddControllerContext(user);
            var result = await controller.PatchAccountData(account.Code,
                new PatchAccountDataBindingModel { Balance = 500, Transactions = new[] { transaction1, transaction2 } });

            Assert.IsInstanceOf<NoContentResult>(result);

            var newAccount = await dbContext.Accounts.Include(x => x.Transactions).SingleOrDefaultAsync(x => x.Id == account.Id);
            Assert.AreEqual(500, newAccount.Balance);
            Assert.AreEqual(2, newAccount.Transactions.Count);
        }

        [Test]
        public async Task PatchAccountData_SaveCalled()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("PatchAccountData_SaveCalled");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            var bank = new Bank() { Title = "Bank #1", User = user };
            dbContext.Banks.Add(bank);
            var account = new BankAccount() { Transactions = new List<Transaction>(), Bank = bank, Code = "Code #1"};
            dbContext.Accounts.Add(account);
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
                    x.Save(It.IsAny<string>(), It.IsAny<BankAccount>(), It.IsAny<float>(), It.IsAny<ICollection<Transaction>>()))
                .Returns(Task.CompletedTask);

            var controller = new AccountsController(dbContext, accountDataSaverMock.Object, _autoMapper.Create(dbContext));
            controller.AddControllerContext(user);
            var result = await controller.PatchAccountData(account.Code,
                new PatchAccountDataBindingModel { Balance = 500, Transactions = new[] { transaction1, transaction2 } });

            Assert.IsInstanceOf<NoContentResult>(result);

            accountDataSaverMock.Verify(x=> 
                x.Save(It.IsAny<string>(), It.IsAny<BankAccount>(), It.IsAny<float>(), It.IsAny<ICollection<Transaction>>()),
                Times.Once);
        }
    }
}