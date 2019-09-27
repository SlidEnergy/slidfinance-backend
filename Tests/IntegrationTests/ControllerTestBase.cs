using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SlidFinance.App;
using SlidFinance.Infrastructure;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SlidFinance.Domain;
using System;

namespace SlidFinance.WebApi.IntegrationTests
{
	public class ControllerTestBase
	{
		protected WebApiApplicationFactory<Startup> _factory;
		protected HttpClient _client;
		protected string _accessToken;
		protected string _refreshToken;
		protected ApplicationDbContext _db;
		protected UserManager<ApplicationUser> _manager;
		protected ApplicationUser _user;
		protected DataAccessLayer _dal;

		[OneTimeSetUp]
		public async Task HttpClientSetup()
		{
			_factory = new WebApiApplicationFactory<Startup>();
			_client = _factory.CreateClient();
			if (_client == null)
				throw new Exception("Клиент для web api не создан.");

			var scope = _factory.Server.Host.Services.CreateScope();

			if (scope == null)
				throw new Exception("Область видимости scope для сервисов не создана.");

			_db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			_manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			if (_db == null)
				throw new Exception("Контекст базы данных не получен");

			if (_manager == null)
				throw new Exception("Сервис для работы с пользователями не получен");
			
			_dal = new DataAccessLayer(
				new EfBanksRepository(_db),
				new EfCategoriesRepository(_db),
				new EfRepository<ApplicationUser, string>(_db),
				new EfBankAccountsRepository(_db),
				new EfRulesRepository(_db),
				new EfTransactionsRepository(_db),
				new EfRefreshTokensRepository(_db));

			_user = new ApplicationUser() { Email = "test1@email.com", UserName = "test1@email.com" };
			var result = await _manager.CreateAsync(_user, "Password123#");
			if (!result.Succeeded)
				throw new Exception("Новый пользователь не создан");

			await Login();
		}

		protected virtual async Task Login()
		{
			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/users/token", null, 
				new { Email = "test1@email.com", Password = "Password123#", ConfirmPassword = "Password123#" });
			var response = await SendRequest(request);

			if (!response.IsSuccessStatusCode)
				throw new Exception("Token для нового пользователя не получен");

			var dict = await response.ToDictionary();

			if (!dict.ContainsKey("token"))
				throw new Exception("Ответ не содержит токен доступа");

			_accessToken = (string)dict["token"];

			if (_accessToken.Length <= 32)
				throw new Exception("Получен невалидный токен");

			if (!dict.ContainsKey("refreshToken"))
				throw new Exception("Ответ не содержит токен восстановления сеанса");

			_refreshToken = (string)dict["refreshToken"];

			if (_refreshToken.Length <= 32)
				throw new Exception("Получен невалидный токен");
		}

		protected virtual Task<HttpResponseMessage> SendRequest(HttpRequestMessage request) => _client.SendAsync(request, CancellationToken.None);
	}
}
