using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.UnitTests
{
	public class MccServiceTests : TestsBase
	{
		MccService _service;

		[SetUp]
		public void Setup()
		{
			_service = new MccService(_db);
		}

		[Test]
		public async Task GetMccList_ShouldBeCallGetListMethod()
		{
			await _db.CreateMcc("4814");
			await _db.CreateMcc("4812");

			var result = await _service.GetListAsync();

			Assert.AreEqual(2, result.Count());
		}


		[Test]
		public async Task ImportWithExistMcc_ShouldNotCallAddMccMethod()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
			var mcc = new Mcc() { Code = "0111" };
			_db.Mcc.Add(mcc);
			await _db.SaveChangesAsync();

			await _service.AddMccIfNotExistAsync(new List<Mcc>() { new Mcc("0111") });
			var mccList = await _db.Mcc.ToListAsync();

			Assert.AreEqual(1, mccList.Count());
		}
	}
}