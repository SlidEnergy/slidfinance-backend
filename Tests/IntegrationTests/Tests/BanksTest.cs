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

			var request = CreateAuthJsonRequest("GET", "/api/v1/banks/");
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
			Assert.NotNull(response.Content);
			var array = await response.ToArrayOfDictionaries();
			Assert.GreaterOrEqual(array.Length, 2);
		}

		[Test]
		public async Task AddBank_ShouldReturnContent()
		{
			var request = CreateAuthJsonRequest("POST", "/api/v1/banks/", new Dto.Bank () {  Title = "Bank #1" });
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

			var request = CreateAuthJsonRequest("PUT", "/api/v1/banks/" + bank.Id, new Dto.Bank
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

			var request = CreateAuthJsonRequest("DELETE", "/api/v1/banks/" + bank.Id);
			var response = await SendRequest(request);

			Assert.True(response.IsSuccessStatusCode);
		}
	}
}
