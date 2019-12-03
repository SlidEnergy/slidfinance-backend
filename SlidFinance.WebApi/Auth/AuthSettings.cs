using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SlidFinance.WebApi
{
	/// <summary>
	/// Параметры для генерации JWT-токена.
	/// </summary>
	public class AuthSettings
    {
		/// <summary>
		/// Издатель токена
		/// </summary>
		public string Issuer { get; set; }

		/// <summary>
		/// Потребитель токена
		/// </summary>
		public string Audience { get; set; }

		/// <summary>
		/// Ключ для шифрования.
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Время жизни токена (в минутах).
		/// </summary>
		public int LifetimeMinutes { get; set; }

		/// <summary>
		/// Получает симметричный ключ безопасности
		/// </summary>
		public SymmetricSecurityKey GetSymmetricSecurityKey()
		{
			return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
		}
	}
}
