using Microsoft.EntityFrameworkCore;
using Moq;
using MyFinanceServer.Api;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class BanksServiceTests
    {
        private readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task AddBank_ShouldBeCallAddMethodWithRightArguments()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("AddBank_ShouldBeCallAddMethodWithRightArguments");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var repository = new Mock<IBanksRepository>();
            repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(user);
            repository.Setup(x => x.Add<Bank>(It.IsAny<Bank>())).ReturnsAsync(new Bank());

            var banksService = new BanksService(repository.Object);

            var category1 = banksService.AddBank(user.Id, "Bank #1");

            repository.Verify(x => x.Add<Bank>(
                It.Is<Bank>(c => c.Title == "Bank #1" && c.User.Id == user.Id)), Times.Exactly(1));
        }

        [Test]
        public async Task DeleteBank_ShouldBeCallAddMethodWithRightArguments()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("DeleteBank_ShouldBeCallAddMethodWithRightArguments");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            var bank = new Bank()
            {
                Title = "Bank #1",
                User = user
            };
            dbContext.Banks.Add(bank);
            await dbContext.SaveChangesAsync();

            var repository = new Mock<IBanksRepository>();
            repository.Setup(x => x.GetById<string, ApplicationUser>(It.IsAny<string>())).ReturnsAsync(user);
            repository.Setup(x => x.GetById<int, Bank>(It.IsAny<int>())).ReturnsAsync(bank);
            repository.Setup(x => x.Delete(It.IsAny<Bank>())).Returns(Task.CompletedTask);

            var banksService = new BanksService(repository.Object);

            await banksService.DeleteBank(user.Id, bank.Id);

            repository.Verify(x => x.Delete(
                It.Is<Bank>(c => c.Title == bank.Title && bank.User.Id == bank.User.Id)),
                Times.Exactly(1));
        }

        [Test]
        public async Task GetBanks_Ok()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("Get_Banks");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            dbContext.Banks.Add(new Bank()
            {
                Title = "Bank #1",
                User = user
            });
            dbContext.Banks.Add(new Bank()
            {
                Title = "Bank #2",
                User = user
            });
            await dbContext.SaveChangesAsync();

            var repository = new Mock<IBanksRepository>();

            var controller = new BanksController(_autoMapper.Create(dbContext), new BanksService(repository.Object));
            controller.AddControllerContext(user);
            var result = await controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }

        [Test]
        public async Task GetBanks_CorrectBalance()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("GetBanks_CorrectBalance");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            dbContext.Banks.Add(new Bank()
            {
                Title = "Bank #1",
                User = user
            });
            dbContext.Banks.Add(new Bank()
            {
                Title = "Bank #2",
                User = user,
                Accounts = new List<BankAccount>() {
                    new BankAccount { Title = "Account #1", Balance = 100},
                    new BankAccount { Title = "Account #2", Balance = 200},
                    new BankAccount { Title = "Account #3", Balance = 300},
                }
            });
            await dbContext.SaveChangesAsync();

            var repository = new Mock<IBanksRepository>();

            var controller = new BanksController(_autoMapper.Create(dbContext), new BanksService(repository.Object));
            controller.AddControllerContext(user);
            var result = await controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }
    }
}