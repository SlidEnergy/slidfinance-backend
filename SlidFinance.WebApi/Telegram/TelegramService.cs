using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	public class TelegramService : ITelegramService
	{
		private IAuthTokenService _authTokenService;
		private TelegramBotSettings _telegramSettings;

		public TelegramService(IAuthTokenService tokenService, TelegramBotSettings telegramSettings)
        {
			_authTokenService = tokenService;
			_telegramSettings = telegramSettings;
		}

		public async Task ConnectTelegramUser(string userId, TelegramUser telegramUser)
		{
			if (!ValidateTelegramInput(telegramUser))
			{
				throw new ArgumentException("Данные телеграм пользователя не прошли проверку.", nameof(telegramUser));
			}

			await _authTokenService.AddToken(userId, telegramUser.Id.ToString(), AuthTokenType.TelegramChatId);
		}

		private bool ValidateTelegramInput(TelegramUser telegramUser)
		{
			var dataCheckString = string.Format("auth_date={0}\nfirst_name={1}\nid={2}\nlast_name={3}\nusername={4}", 
				telegramUser.Auth_date, telegramUser.First_name, telegramUser.Id, telegramUser.Last_name, telegramUser.Username);

			using (var sha256 = SHA256.Create())
			{
				var secretKey = sha256.ComputeHash(Encoding.UTF8.GetBytes(_telegramSettings.Token));

				using (var hmac = new HMACSHA256(secretKey))
				{
					byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));

					if (BitConverter.ToString(hashValue).Replace("-", "").ToLower() == telegramUser.Hash)
						return true;
				}
			}

			return false;
		}
	}
}
