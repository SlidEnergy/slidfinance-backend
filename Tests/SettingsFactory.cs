namespace MyFinanceServer.Tests
{
	/// <summary>
	/// Создает объекты с настройками приложения.
	/// </summary>
	public class SettingsFactory
	{
		public static AuthSettings CreateAuth()
		{
			return new AuthSettings() {
				Issuer = "Test issuer",
				Audience = "Test audience",
				Key = "Very very very long secret #1",
				LifetimeMinutes = 60
			};
		}
	}
}
