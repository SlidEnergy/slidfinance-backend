using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	public interface ITelegramService
	{
		Task ConnectTelegramUser(string userId, TelegramUser telegramUser);
	}
}