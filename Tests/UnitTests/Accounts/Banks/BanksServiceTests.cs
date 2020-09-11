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
			_service = new BanksService(_db);
		}

		[Test]
		public async Task AddBank_ShouldCallAddMethodWithRightArguments()
		{
			var category1 = await _service.AddBank("Bank #1");

			Assert.IsTrue(_db.Banks.Any(x => x.Title == "Bank #1"));
		}

		[Test]
		public async Task DeleteBank_ShouldCallAddMethodWithRightArguments()
		{
			var bank = await _db.CreateBank();

			await _service.DeleteBank(bank.Id);

			Assert.IsFalse(_db.Banks.Any(x => x.Id == bank.Id));
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