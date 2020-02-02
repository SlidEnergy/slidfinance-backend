using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SlidFinance.App;
using SlidFinance.Domain;
using SlidFinance.Infrastructure;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

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

			var scope = _factory.Services.CreateScope();

			if (scope == null)
				throw new Exception("Область видимости scope для сервисов не создана.");

			_db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			_manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			if (_db == null)
				throw new Exception("Контекст базы данных не получен");

			if (_manager == null)
				throw new Exception("Сервис для работы с пользователями не получен");
			
			_dal = new DataAccessLayer(
				new EfRepository<Bank, int>(_db),
				new EfRepository<UserCategory,int>(_db),
				new EfRepository<ApplicationUser, string>(_db),
				new EfRepository<BankAccount, int>(_db),
				new EfRepository<Rule, int>(_db),
				new EfRepository<Transaction, int>(_db),
				new EfAuthTokensRepository(_db),
				new EfRepository<Mcc, int>(_db));

			_user = await CreateUser("test1@email.com", "Password123#");

			await Login("test1@email.com", "Password123#");
		}

		protected virtual async Task<ApplicationUser> CreateUser(string email, string password)
		{
			var user = new ApplicationUser() { Email = email, UserName = email, Trustee = new Trustee() };
			var result = await _manager.CreateAsync(user, password);
			if (!result.Succeeded)
				throw new Exception("Новый пользователь не создан");

			return user;
		}

		protected virtual async Task Login(string email, string password)
		{
			var request = HttpRequestBuilder.CreateJsonRequest("POST", "/api/v1/users/token", null, 
				new { Email = email, Password = password, ConfirmPassword = password });
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
