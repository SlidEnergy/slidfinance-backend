using MihaZupan;
using SlidFinance.TelegramBot.Models;
using SlidFinance.TelegramBot.Models.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;

namespace SlidFinance.TelegramBot
{
	public class BotService : IBotService
	{
		private BotSettings _settings;

		private List<Command> commandsList;

		public IReadOnlyList<Command> Commands => commandsList.AsReadOnly();

		public TelegramBotClient Client { get; private set; }

		public BotService(BotSettings settings) 
		{
			_settings = settings;
		}

		public async Task InitAsync()
		{
			if(_settings != null && !string.IsNullOrEmpty(_settings.Key) && !string.IsNullOrEmpty(_settings.Url))
				Client = await CreateBotClientAsync(_settings);
		}

		private async Task<TelegramBotClient> CreateBotClientAsync(BotSettings settings)
		{
			var client = string.IsNullOrEmpty(settings.Socks5Host)
				? new TelegramBotClient(settings.Key)
				: new TelegramBotClient(settings.Key, new HttpToSocks5Proxy(settings.Socks5Host, settings.Socks5Port));
			await client.SetWebhookAsync(settings.Url + "/api/update");
			return client;
		}
	}
}
