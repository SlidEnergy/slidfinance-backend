using Microsoft.AspNetCore.Identity;
using SlidFinance.Domain;
using SlidFinance.WebApi;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class TelegramService : ITelegramService
	{
        private readonly UserManager<ApplicationUser> _userManager;
		private TelegramBotSettings _telegramSettings;
		private DataAccessLayer _dal;

		public TelegramService(UserManager<ApplicationUser> userManager, TelegramBotSettings telegramSettings, DataAccessLayer dal)
        {
			_userManager = userManager;
			_telegramSettings = telegramSettings;
			_dal = dal;
		}

		public async Task ConnectTelegramUser(string userId, TelegramUser telegramUser)
		{
			if (!ValidateTelegramInput(telegramUser))
			{
				throw new ArgumentException("Данные телеграм пользователя не прошли проверку.", nameof(telegramUser));
			}

			var user = await _userManager.FindByIdAsync(userId);

			var token = await _dal.AuthTokens.FindAnyToken(telegramUser.Id.ToString());

			if (token == null || token.Type != AuthTokenType.TelegramChatId || token.UserId != user.Id)
			{
				await _dal.AuthTokens.Add(new AuthToken("any", telegramUser.Id.ToString(), user, AuthTokenType.TelegramChatId));
			}
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
