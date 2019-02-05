using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class BankTests
    {
        private readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();

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

            var controller = new BanksController(dbContext, _autoMapper.Create());
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

            var controller = new BanksController(dbContext, _autoMapper.Create());
            controller.AddControllerContext(user);
            var result = await controller.GetList();

            Assert.AreEqual(2, result.Value.Count());
        }
    }
}