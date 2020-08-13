using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SlidFinance.App
{
	/// <summary>
	/// Класс расширений для ClaimsPrincipal, содержит методы получения данных из Claims.
	/// </summary>
    public static class ClaimsPrincipalExtensions
    {
		/// <summary>
		/// Получает идентификатор пользователя из Claims.
		/// </summary>
		/// <param name="user">Пользователь</param>
        public static string GetUserId(this ClaimsPrincipal user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            return user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
