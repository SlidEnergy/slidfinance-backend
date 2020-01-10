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

		private async Task<string> GetRefreshToken() 
		{
			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/import/refreshtoken", _accessToken);
			var response = await SendRequest(request);

			response.IsSuccess();
			Assert.NotNull(response.Content);
			return await response.ToJsonString();
		}

		[Test]
		public async Task GetToken_ShouldReturnTokens()
		{
			var refreshToken = await GetRefreshToken();
			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/import/token", null, new TokensCortage { RefreshToken = refreshToken });
			var response = await SendRequest(request);

			response.IsSuccess();
			Assert.NotNull(response.Content);
			var newTokenCortage = await response.ToObject<TokensCortage>();
		}

		[Test]
		public async Task GetRefreshToken_ShouldReturnTokens()
		{
			var refreshToken = await GetRefreshToken();
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

			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/import", _accessToken,
				new PatchAccountDataBindingModel()
				{
					Code = account.Code,
					Balance = 100,
					Transactions = new Dto.ImportTransaction[] {
						new Dto.ImportTransaction() { Category = "Bank category #1", Description = "Description #1", DateTime = DateTime.Today },
						new Dto.ImportTransaction() { Category = "Bank category #2", Description = "Description #2", DateTime = DateTime.Today }
					}
				});

			var response = await SendRequest(request);

			response.IsSuccess();
		}
	}
}
