using Moq;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using SlidFinance.App.Saltedge;
using SaltEdgeNetCore.Client;
using SaltEdgeNetCore.Models.Transaction;
using SaltEdgeNetCore.Models.Responses;
using SaltEdgeNetCore.Models.Extra;
using SaltEdgeNetCore.Models.Connections;
using SaltEdgeNetCore.Models.Account;

namespace SlidFinance.WebApi.UnitTests
{
	public class SaltedgeServiceTests : TestsBase
	{
		SaltedgeService _service;
		Mock<IMccService> _mccService;
		Mock<IImportService> _importService;
		Mock<IMerchantService> _merchantService;
		Mock<IAccountsService> _accountService;
		Mock<ISaltEdgeClientV5> _saltedgeClient;

		[SetUp]
		public void Setup()
		{
			_importService = new Mock<IImportService>();
			_mccService = new Mock<IMccService>();
			_merchantService = new Mock<IMerchantService>();
			_accountService = new Mock<IAccountsService>();
			_saltedgeClient = new Mock<ISaltEdgeClientV5>();

			_service = new SaltedgeService(_db, _saltedgeClient.Object, _importService.Object, _mccService.Object, _merchantService.Object);
		}

		[Test]
		public async Task Import_ShouldCallMethodWithRightParameters()
		{
			var bank = new Bank() { Title = "Bank #1" };
			_db.Banks.Add(bank);
			var account = await _db.CreateAccount(_user);
			account.SaltedgeBankAccountId = "SaltedgeBankAccountId";
			var category = await _db.CreateCategory(_user);
			var mcc = new Mcc() { Code = "0111" };
			_db.Mcc.Add(mcc);
			var saltedgeAccount = new SaltedgeAccount() { Id = 1, CustomerId = "CustomerId" };
			_db.SaltedgeAccounts.Add(saltedgeAccount);
			_db.TrusteeSaltedgeAccounts.Add(new TrusteeSaltedgeAccount(_user, saltedgeAccount.Id));
			await _db.SaveChangesAsync();
			var transaction1 = new SaltEdgeTransaction()
			{
				MadeOn = DateTime.Now,
				Amount = 10,
				Description = "Description #1",
				Category = "Bank category #1",
				Extra = new SeTransactionExtra()
				{
					Additional = "SAMBERY 323MCC: 0111",
					AccountBalanceSnapshot = 100
				}
			};
			var transactionsResponse = new SaltEdgeNetCore.Models.Responses.Response<IEnumerable<SaltEdgeTransaction>, SePaging>();
			transactionsResponse.Data = new List<SaltEdgeTransaction>() { transaction1 };

			var connectionsResponse = new SaltEdgeNetCore.Models.Responses.Response<IEnumerable<SeConnection>, SePaging>();
			connectionsResponse.Data = new List<SeConnection>() { new SeConnection() { Id = "ConnectionId" } };

			var accountsResponse = new SaltEdgeNetCore.Models.Responses.Response<IEnumerable<SeAccount>, SePaging>();
			accountsResponse.Data = new List<SeAccount>() { new SeAccount() { Id = "SaltedgeBankAccountId" } };

			_saltedgeClient.Setup(x => x.TransactionsList(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(transactionsResponse);
			_saltedgeClient.Setup(x => x.ConnectionsList(It.IsAny<string>(), It.IsAny<string>())).Returns(connectionsResponse);
			_saltedgeClient.Setup(x => x.AccountList(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(accountsResponse);
			_importService.Setup(x => x.Import(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<float?>(), It.IsAny<Transaction[]>())).ReturnsAsync(1);
			_mccService.Setup(x => x.GetListAsync()).ReturnsAsync(new List<Mcc>() { mcc });
			_mccService.Setup(x => x.AddAsync(It.IsAny<Mcc>())).ReturnsAsync(mcc);
			_mccService.Setup(x => x.AddMccIfNotExistAsync(It.IsAny<ICollection<Mcc>>())).Returns(Task.CompletedTask);
			_merchantService.Setup(x => x.GetListAsync()).ReturnsAsync(new List<Merchant>());
			_merchantService.Setup(x => x.AddAsync(It.IsAny<Merchant>())).ReturnsAsync((Merchant x) => x);

			var result = await _service.Import(_user.Id);

			_importService.Verify(x => x.Import(It.Is<string>(u => u == _user.Id), It.Is<int>(a => a == account.Id), It.Is<float?>(b => b == (float?)transaction1.Extra.AccountBalanceSnapshot),
				It.Is<Transaction[]>(t =>
					t[0].BankCategory == transaction1.Category &&
					t[0].DateTime == transaction1.MadeOn &&
					t[0].Amount == (float)transaction1.Amount &&
					t[0].Description == "SAMBERY 323" &&
					t[0].MccId == mcc.Id
					)));
		}
	}
}
