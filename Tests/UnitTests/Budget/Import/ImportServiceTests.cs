using Moq;
using SlidFinance.App;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SlidFinance.WebApi.UnitTests
{
	public class ImportServiceTests : TestsBase
	{
		private ImportService _service;

		[SetUp]
		public void Setup()
		{
			_service = new ImportService(_db);
		}

		[Test]
		public async Task ImportWithExistsRule_ShouldCallMethodWithRightCategory()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
			var rule = new Rule() { Category = category, Account = account };
			_db.Rules.Add(rule);

			var transaction = new Transaction()
			{
				DateTime = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				BankCategory = "Bank category #1",
				Approved = false,
				MccId = 111
			};

			await _db.SaveChangesAsync();

			var count = await _service.Import(_user.Id, account.Id, 100, new Transaction[] { transaction });

			var list = await _db.Transactions.Where(t => t.CategoryId == category.Id && t.AccountId == account.Id).ToListAsync();

			Assert.AreEqual(1, count);
			Assert.AreEqual(1, list.Count);
		}

		[Test]
		public async Task Import_ShouldBeAdded()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
			var transaction1 = new Transaction()
			{
				DateTime = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				BankCategory = "Bank category #1",
				Approved = false,
				MccId = 111
			};

			var count = await _service.Import(_user.Id, account.Id, 100, new Transaction[] { transaction1 });

			var newTransaction = await _db.Transactions.FirstOrDefaultAsync(t => t.AccountId == account.Id);
			Assert.NotNull(newTransaction);
		}

		[Test]
		public async Task ImportWithoutMcc_ShouldBeAdded()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
			var transaction1 = new Transaction()
			{
				DateTime = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				BankCategory = "Bank category #1",
			};

			var count = await _service.Import(_user.Id, account.Id, 100, new Transaction[] { transaction1 });

			var newTransaction = await _db.Transactions.FirstOrDefaultAsync(t => t.AccountId == account.Id);
			Assert.NotNull(newTransaction);
		}

		[Test]
		public async Task Import_ShouldCallAddMethodTwice()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
			var transaction1 = new Transaction()
			{
				DateTime = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				BankCategory = "Bank category #1",
				Approved = false,
				MccId = 111
			};
			var transaction2 = new Transaction()
			{
				DateTime = DateTime.Now,
				Amount = 5,
				Description = "Description #2",
				BankCategory = "Bank category #1",
				Approved = false,
				MccId = 111
			};

			var count = await _service.Import(_user.Id, account.Id, 100, new Transaction[] { transaction1, transaction2 });

			var list = await _db.Transactions.Where(t => t.AccountId == account.Id).ToListAsync();
			Assert.AreEqual(2, count);
			Assert.AreEqual(2, list.Count);
		}

		[Test]
		public async Task ImportDublicates_ShouldNotImported()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = _db.CreateCategory(_user);

			var transaction = new Transaction()
			{
				DateTime = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				BankCategory = "Bank category #1",
				Approved = false,
				MccId = 111
			};

			_db.Transactions.Add(new Transaction() { DateTime = transaction.DateTime, Amount = transaction.Amount, Description = transaction.Description,
				Account = account });

			await _db.SaveChangesAsync();

			var count = await _service.Import(_user.Id, account.Id, 100, new Transaction[] { transaction });

			var list = await _db.Transactions.Where(t => t.AccountId == account.Id).ToListAsync();
			Assert.AreEqual(0, count);
			Assert.AreEqual(1, list.Count);
		}
	}
}