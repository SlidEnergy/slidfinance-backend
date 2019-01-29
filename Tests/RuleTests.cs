using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Api;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class RuleTests
    {
        private readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public async Task GetRules_Ok()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("GetRules_Ok");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            var bank = new Bank() { Title = "Bank #1", User = user };
            dbContext.Banks.Add(bank);
            var account = new BankAccount() { Title = "Account #1", Bank = bank };
            dbContext.Accounts.Add(account);
            var category = new Category() {Title = "Category #1", User = user};
            var rule1 = new Rule()
            {
                Account = account, BankCategory = "Category #1", Category = category, Description = "Description #1",
                Mcc = 5555, Order = 1
            };
            var rule2 = new Rule()
            {
                Account = account,
                BankCategory = "Category #1",
                Category = category,
                Description = "Description #1",
                Mcc = 3333,
                Order = 2
            };
            dbContext.Rules.AddRange(new Rule[] { rule1, rule2 });
            // testDbContext.addUser(1).addBank(3).addAccount(3)
            // users.addCategory(3)
            // before.addRule(1)

            await dbContext.SaveChangesAsync();

            var controller = new RulesController(dbContext, _autoMapper.Create());
            controller.AddControllerContext(user);
            var result = await controller.GetRule();

            Assert.AreEqual(2, result.Value.Count());
        }

        [Test]
        public async Task AddRule_Ok()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("GetRules_Ok");
            var dbContext = new ApplicationDbContext(optionsBuilder.Options);
            var user = new ApplicationUser() { Email = "Email #1" };
            dbContext.Users.Add(user);
            var bank = new Bank() { Title = "Bank #1", User = user };
            dbContext.Banks.Add(bank);
            var account = new BankAccount() { Title = "Account #1", Bank = bank };
            dbContext.Accounts.Add(account);
            var category = new Category() { Title = "Category #1", User = user };
            dbContext.Categories.Add(category);
            var rule = new Rule()
            {
                Account = account,
                BankCategory = "Category #1",
                Category = category,
                Description = "Description #1",
                Mcc = 5555
            };

            // testDbContext.addUser(1).addBank(3).addAccount(3)
            // users.addCategory(3)

            await dbContext.SaveChangesAsync();

            var controller = new RulesController(dbContext, _autoMapper.Create());
            controller.AddControllerContext(user);
            var result = await controller.PostRule(new Api.Dto.Rule()
            {
                AccountId = rule.Account.Id,
                BankCategory = rule.BankCategory,
                CategoryId = rule.Category.Id,
                Description = rule.Description,
                Mcc = rule.Mcc
            });

            var newRule = await dbContext.Rules.Include(x=>x.Account).Include(x=>x.Category)
                .SingleOrDefaultAsync(x=>x.Id == ((Api.Dto.Rule)((CreatedAtActionResult)result.Result).Value).Id);
            Assert.AreEqual(rule.Account.Id, newRule.Account.Id);
            Assert.AreEqual(rule.Category.Id, newRule.Category.Id);
            Assert.AreEqual(rule.BankCategory, newRule.BankCategory);
            Assert.AreEqual(rule.Description, newRule.Description);
            Assert.AreEqual(rule.Mcc, newRule.Mcc);
            Assert.AreEqual(1, newRule.Order);
        }

      
    }
}