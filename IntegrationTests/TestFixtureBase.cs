using NUnit.Framework;
using System.Threading.Tasks;

namespace MyFinanceServer.IntegrationTests
{
	/// <summary>
	/// Базовый класс для тестов Lers Framework. Получает подключение к тестовому серверу.
	/// </summary>
	public class TestFixtureBase
	{
		/// <summary>
		/// Инициализация тестов
		/// </summary>
		[OneTimeSetUp]
		public async Task Init()
		{
		}

		//protected virtual T CreateController<T>() where T : ControllerBase
		//{
		//	var userMock = MockRepository.GenerateMock<IAppUser>();
		//	userMock.Stub(x => x.Server).Return(this.Server);
		//	userMock.Stub(x => x.Reports).Return(this.Reports);

		//	var managerMock = MockRepository.GenerateMock<IUserManager>();
		//	managerMock.Stub(x => x.User).Return(userMock);
		//	managerMock.Stub(x => x.Server).Return(userMock.Server);
		//	managerMock.Stub(x => x.Reports).Return(userMock.Reports);

		//	var loggerMock = MockRepository.GenerateMock<ILogger>();

		//	var controller = (T)Activator.CreateInstance(typeof(T), managerMock, loggerMock);
		//	var request = new HttpRequestMessage();
		//	request.SetConfiguration(new HttpConfiguration());

		//	controller.Request = request;

		//	return controller;
		//}

		/// <summary>
		/// Очистка ресурсов тестов
		/// </summary>
		[OneTimeTearDown]
		public void Release()
		{
		}
	}
}
