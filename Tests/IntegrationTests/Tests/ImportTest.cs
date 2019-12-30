using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.IntegrationTests
{
	[TestFixture]
	public class ImportTest : ControllerTestBase
	{
		[SetUp]
		public void Setup()
		{

		}

		[Test]
		public async Task GetToken_ShouldReturnTokens()
		{
			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/import/token", _accessToken);
			var response = await SendRequest(request);

			response.IsSuccess();
			Assert.NotNull(response.Content);
			var tokenCortage = await response.ToObject<TokensCortage>();
		}

		[Test]
		public async Task Import_ShouldReturnOk()
		{
			var bank = new Bank("Bank #1");
			await _dal.Banks.Add(bank);
			var account = new BankAccount(bank, "Account #1", "Code #1", 100, 50);
			await _dal.Accounts.Add(account);
			_db.TrusteeAccounts.Add(new TrusteeAccount(_user, account));
			await _db.SaveChangesAsync();

			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/import/token", _accessToken);
			var response = await SendRequest(request);
			response.IsSuccess();
			Assert.NotNull(response.Content);
			var tokenCortage = await response.ToObject<TokensCortage>();

			request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/import", tokenCortage.Token,
				new PatchAccountDataBindingModel()
				{
					Code = account.Code,
					Balance = 100,
					Transactions = new Dto.ImportTransaction[] {
						new Dto.ImportTransaction() { Category = "Bank category #1", Description = "Description #1", DateTime = DateTime.Today },
						new Dto.ImportTransaction() { Category = "Bank category #2", Description = "Description #2", DateTime = DateTime.Today }
					}
				});

			response = await SendRequest(request);

			response.IsSuccess();
		}
	}
}
