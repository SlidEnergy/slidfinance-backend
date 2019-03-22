using Microsoft.EntityFrameworkCore;
using MyFinanceServer.Core;
using MyFinanceServer.Data;
using NUnit.Framework;
using System.Threading.Tasks;

namespace MyFinanceServer.Tests
{
    public class TestsBase
    {
        protected readonly AutoMapperFactory _autoMapper = new AutoMapperFactory();
        protected ApplicationDbContext _db;
        protected DataAccessLayer _dal;
        protected ApplicationUser _user;

        [SetUp]
        public async Task SetupBase()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseInMemoryDatabase(TestContext.CurrentContext.Test.Name);
            _db = new ApplicationDbContext(optionsBuilder.Options);
            _dal = new DataAccessLayer(new EfBanksRepository(_db), new EfCategoriesRepository(_db), new EfRepository(_db));
            _user = await _dal.Users.Add(new ApplicationUser() { Email = "Email #1" });
        }
    }
}
