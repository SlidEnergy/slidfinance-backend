using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SlidFinance.WebApi.UnitTests
{
	public class ApiImportServiceTests : TestsBase
    {
		ApiImportService _service;
		Mock<IMccService> _mccService;
		Mock<IImportService> _importService;
		Mock<IMerchantService> _merchantService;
		Mock<IAccountsService> _accountService;

		[SetUp]
        public void Setup()
        {
			_importService = new Mock<IImportService>();
			_mccService = new Mock<IMccService>();
			_merchantService = new Mock<IMerchantService>();
			_accountService = new Mock<IAccountsService>();

			_service = new ApiImportService(_autoMapper.Create(_db), _importService.Object, _mccService.Object, _merchantService.Object, _accountService.Object);
		}

		[Test]
		public async Task Import_ShouldCallMethodWithRightParameters()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
			var mcc = new Mcc() { Code = "0111" };
			_db.Mcc.Add(mcc);
			await _db.SaveChangesAsync();
			var transaction1 = new Dto.ImportTransaction()
			{
				DateTime = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				Category = "Bank category #1",
				Mcc = 111
			};

			_importService.Setup(x => x.Import(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<float?>(), It.IsAny<Transaction[]>())).ReturnsAsync(1);
			_mccService.Setup(x => x.GetListAsync()).ReturnsAsync(new List<Mcc>() { mcc });
			_mccService.Setup(x => x.AddAsync(It.IsAny<Mcc>())).ReturnsAsync(mcc);
			_merchantService.Setup(x => x.GetListAsync()).ReturnsAsync(new List<Merchant>());
			_merchantService.Setup(x => x.AddAsync(It.IsAny<Merchant>())).ReturnsAsync((Merchant x) => x);

			var result = await _service.Import(_user.Id, new PatchAccountDataBindingModel() { Code = account.Code, Balance = 100, Transactions = new Dto.ImportTransaction[] { transaction1 } });

			_importService.Verify(x => x.Import(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<float?>(), 
				It.Is<Transaction[]>(t => t[0].BankCategory == transaction1.Category)));
		}

		[Test]
		public async Task ImportWithNewMcc_ShouldCallAddMccMethod()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			var category = await _db.CreateCategory(_user);
			var mcc = new Mcc() { Code = "0111" };
			_db.Mcc.Add(mcc);
			await _db.SaveChangesAsync();
			var transaction1 = new Dto.ImportTransaction()
			{
				DateTime = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				Category = "Bank category #1",
				Mcc = 111
			};

			_importService.Setup(x => x.Import(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<float?>(), It.IsAny<Transaction[]>())).ReturnsAsync(1);
			var queue = new Queue<List<Mcc>>();
			queue.Enqueue(new List<Mcc>());
			queue.Enqueue(new List<Mcc>() { mcc });
			_mccService.Setup(x => x.GetListAsync()).ReturnsAsync(queue.Dequeue);
			_mccService.Setup(x => x.AddAsync(It.IsAny<Mcc>())).ReturnsAsync(mcc);
			_merchantService.Setup(x => x.GetListAsync()).ReturnsAsync(new List<Merchant>());
			_merchantService.Setup(x => x.AddAsync(It.IsAny<Merchant>())).ReturnsAsync((Merchant x) => x);

			var result = await _service.Import(_user.Id, new PatchAccountDataBindingModel() { Code = account.Code, Balance = 100, Transactions = new Dto.ImportTransaction[] { transaction1 } });

			_mccService.Verify(x => x.AddAsync(It.Is<Mcc>(m => m.Code == transaction1.Mcc.Value.ToString("D4") && m.IsSystem == false)));
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
			var transaction1 = new Dto.ImportTransaction()
			{
				DateTime = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				Category = "Bank category #1",
				Mcc = 111
			};
			
			_importService.Setup(x => x.Import(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<float?>(), It.IsAny<Transaction[]>())).ReturnsAsync(1);
			_mccService.Setup(x => x.GetListAsync()).ReturnsAsync(new List<Mcc>() { mcc });
			_mccService.Setup(x => x.AddAsync(It.IsAny<Mcc>())).ReturnsAsync((Mcc x) => x);
			_merchantService.Setup(x => x.GetListAsync()).ReturnsAsync(new List<Merchant>());
			_merchantService.Setup(x => x.AddAsync(It.IsAny<Merchant>())).ReturnsAsync((Merchant x) => x);

			var result = await _service.Import(_user.Id, new PatchAccountDataBindingModel() { Code = account.Code, Balance = 100, Transactions = new Dto.ImportTransaction[] { transaction1 } });

			_mccService.Verify(x => x.AddAsync(It.IsAny<Mcc>()), Times.Never);
		}
	}
}
