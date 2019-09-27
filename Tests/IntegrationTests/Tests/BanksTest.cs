using NUnit.Framework;
using SlidFinance.Domain;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.IntegrationTests
{
	[TestFixture]
	public class BanksTest : ControllerTestBase
	{
		[Test]
		public async Task GetBanksList_ShouldReturnContent()
		{
			var bank1 = new Bank("Bank #1", _user);
			await _dal.Banks.Add(bank1);
			var bank2 = new Bank("Bank #2", _user);
			await _dal.Banks.Add(bank2);

			var request = HttpRequestBuilder.CreateJsonRequest("GET", "/api/v1/banks/", _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}

		[Test]
		public async Task AddBank_ShouldReturnContent()
		{
			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/banks/", _accessToken, new Dto.Bank () {  Title = "Bank #1" });
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task UpdateBank_ShouldReturnContent()
		{
			var bank = new Bank("Bank #1", _user);
			await _dal.Banks.Add(bank);

			var request = HttpRequestBuilder.CreateJsonRequest("PUT", "/api/v1/banks/" + bank.Id, _accessToken, new Dto.Bank
			{
				Id = bank.Id,
				Title = "Bank #2"
			});
			
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var dict = await response.ToDictionary();
			Assert.IsTrue(dict.ContainsKey("id"));
		}

		[Test]
		public async Task DeleteBank_ShouldNoContent()
		{
			var bank = new Bank("Bank #1", _user);
			await _dal.Banks.Add(bank);

			var request = HttpRequestBuilder.CreateJsonRequest("DELETE", "/api/v1/banks/" + bank.Id, _accessToken);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
		}
	}
}
