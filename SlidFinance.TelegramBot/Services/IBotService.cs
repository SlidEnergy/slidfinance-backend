using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.TelegramBot.Models.Commands;
using Telegram.Bot;

namespace SlidFinance.TelegramBot
{
	public interface IBotService
	{
		TelegramBotClient Client { get; }
		IReadOnlyList<Command> Commands { get; }
		Task InitAsync();
	}
}