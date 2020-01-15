using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SlidFinance.TelegramBot.Models.Commands
{
	public class WhichCardToPayCommand : Command
	{
		public override string Name => @"/whichtopay";

		public override bool Contains(Message message)
		{
			if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
				return false;

			return message.Text.Contains(this.Name);
		}

		public override async Task Execute(Message message, TelegramBotClient botClient)
		{
			var chatId = message.Chat.Id;
			await botClient.SendTextMessageAsync(chatId, "Введите название категории, магазина или mcc код.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);


		}
	}
}
