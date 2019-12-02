using System.Threading.Tasks;
using Telegram.Bot;

namespace SlidFinance.TelegramBot
{
	public interface IBotService
	{
		TelegramBotClient Client { get; }
		Task InitAsync();
	}
}