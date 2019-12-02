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

		public TelegramService(UserManager<ApplicationUser> userManager, TelegramBotSettings telegramSettings)
        {
			_userManager = userManager;
			_telegramSettings = telegramSettings;
		}

		public async Task ConnectTelegramUser(string userId, TelegramUser telegramUser)
		{
			if (!ValidateTelegramInput(telegramUser))
			{
				throw new ArgumentException("Данные телеграм пользователя не прошли проверку.", nameof(telegramUser));
			}

			var user = await _userManager.FindByIdAsync(userId);
			user.Telegram = new UserTelegramRelation() { TelegramChatId = telegramUser.Id, UserId = user.Id };
			await _userManager.UpdateAsync(user);
		}

		private bool ValidateTelegramInput(TelegramUser telegramUser)
		{
			var dataCheckString = string.Format("auth_date={0}\nfirst_name={1}\nid={2}\nusername={3}", 
				telegramUser.Auth_date, telegramUser.First_name, telegramUser.Id, telegramUser.Username);

			using (var sha256 = SHA256.Create())
			{
				var secretKey = sha256.ComputeHash(Encoding.UTF8.GetBytes(_telegramSettings.Token));

				using (var hmac = new HMACSHA256(secretKey))
				{
					byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataCheckString));

					if (BitConverter.ToString(hashValue).Replace("-", "") == telegramUser.Hash)
						return true;
				}
			}

			return false;
		}
	}
}
