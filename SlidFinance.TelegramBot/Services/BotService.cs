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
		private TelegramBotSettings _settings;

		private List<Command> commandsList;

		public IReadOnlyList<Command> Commands => commandsList.AsReadOnly();

		public TelegramBotClient Client { get; private set; }

		public BotService(TelegramBotSettings settings) 
		{
			_settings = settings;
		}

		public async Task InitAsync()
		{
			if(_settings != null && !string.IsNullOrEmpty(_settings.Token) && !string.IsNullOrEmpty(_settings.Url))
				Client = await CreateBotClientAsync(_settings);
		}

		private async Task<TelegramBotClient> CreateBotClientAsync(TelegramBotSettings settings)
		{
			var client = string.IsNullOrEmpty(settings.Socks5Host)
				? new TelegramBotClient(settings.Token)
				: new TelegramBotClient(settings.Token, new HttpToSocks5Proxy(settings.Socks5Host, settings.Socks5Port));
			await client.SetWebhookAsync(settings.Url + "/api/update");
			return client;
		}
	}
}
