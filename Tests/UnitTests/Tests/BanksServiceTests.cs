using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.WebApi.UnitTests
{
    public class BanksServiceTests : TestsBase
    {
        private BanksService _service;

        [SetUp]
        public void Setup()
        {
            _service = new BanksService(_mockedDal, _db);
        }

        [Test]
        public async Task AddBank_ShouldCallAddMethodWithRightArguments()
        {
            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _banks.Setup(x => x.Add(It.IsAny<Bank>())).ReturnsAsync(new Bank());

            var category1 = await _service.AddBank("Bank #1");

            _banks.Verify(x => x.Add(
                It.Is<Bank>(c => c.Title == "Bank #1")), Times.Exactly(1));
        }

        [Test]
        public async Task DeleteBank_ShouldCallAddMethodWithRightArguments()
        {
            var bank = await _dal.Banks.Add(new Bank()
            {
                Title = "Bank #1",
            });

            _users.Setup(x => x.GetById(It.IsAny<string>())).ReturnsAsync(_user);
            _banks.Setup(x => x.GetById(It.IsAny<int>())).ReturnsAsync(bank);
            _banks.Setup(x => x.Delete(It.IsAny<Bank>())).Returns(Task.CompletedTask);

            await _service.DeleteBank(bank.Id);

            _banks.Verify(x => x.Delete(
                It.Is<Bank>(c => c.Title == bank.Title)),
                Times.Exactly(1));
        }

        [Test]
        public async Task GetBanks_ShouldReturnList()
        {
			var bank1 = new Bank()
			{
				Title = "Bank #1",
			};
			_db.Banks.Add(bank1);
			var bank2 = new Bank()
			{
				Title = "Bank #2",
			};
			_db.Banks.Add(bank2);
			await _db.SaveChangesAsync();

            var result = await _service.GetLis();
            
            Assert.AreEqual(2, result.Count);
        }
    }
}