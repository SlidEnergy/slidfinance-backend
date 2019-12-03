using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace SlidFinance.TelegramBot
{
	public interface IUpdateService
	{
		Task EchoAsync(Update update);
	}
}