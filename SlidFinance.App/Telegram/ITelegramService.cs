using System.Threading.Tasks;

namespace SlidFinance.App
{
	public interface ITelegramService
	{
		Task ConnectTelegramUser(string userId, TelegramUser telegramUser);
	}
}