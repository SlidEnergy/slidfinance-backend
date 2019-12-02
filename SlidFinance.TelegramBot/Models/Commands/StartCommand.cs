using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SlidFinance.TelegramBot.Models.Commands
{
	public class StartCommand : Command
	{
		public override string Name => @"/start";
		
		private IMemoryCache _cache;

		public StartCommand(IMemoryCache cache)
		{
			_cache = cache;
		}

		public override bool Contains(Message message)
		{
			if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
				return false;

			return message.Text.Contains(this.Name);
		}

		public override async Task Execute(Message message, TelegramBotClient botClient)
		{
			var chatId = message.Chat.Id;

			//if (message.Text.Length > Name.Length && message.Text[Name.Length] == ' ')
			//{
			//	var token = message.Text.Split(" ")[1];
			//	_cache.Set(chatId, token);
			//	await botClient.SendTextMessageAsync(chatId, "Спасибо", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
			//	return;
			//}

			//await botClient.SendTextMessageAsync(chatId, "Приветствую! Я ваш финансовый помошник. Перейдите по ссылке в настройке профиля, чтобы авторизоваться. [Профиль](https://slidfinance-frontend.herokuapp.com/profile/settings)", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
			await botClient.SendTextMessageAsync(chatId, "Приветствую! Я ваш финансовый помошник.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
		}
	}
}
