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

            // При восстановлении Claims, библиотека jwt заменяет наши ClaimType на свои (как это перенастроить?), 
            // а при создании через ApiKey добавляется только один новый ClaimType, не изменяя старые.
            // Поэтому идентификатор пользователя может быть в разных полях.
            
            var claim = user.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
                return claim.Value;

            claim = user.FindFirst(JwtRegisteredClaimNames.Sub);

            return claim?.Value;
        }
    }
}
