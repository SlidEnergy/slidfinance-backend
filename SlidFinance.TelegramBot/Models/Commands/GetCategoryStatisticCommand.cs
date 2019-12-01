using SlidFinance.App;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace SlidFinance.TelegramBot.Models.Commands
{
	public class GetCategoryStatisticCommand : Command
	{
		public override string Name => @"/getcategorystatistic";

		private IUsersService _usersService;
		private ICategoryStatisticService _categoryStatisticService;

		public GetCategoryStatisticCommand(IUsersService usersService, ICategoryStatisticService categoryStatisticService)
		{
			_usersService = usersService;
			_categoryStatisticService = categoryStatisticService;
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

			var user = await _usersService.GetByTelegramChatIdAsync(chatId);

			if (user == null)
			{
				await botClient.SendTextMessageAsync(chatId, "Пользователь не авторизован. Войдите в систему, перейдите на вкладку \"Настройки\" и нажмите привязать аккаунт Телеграм.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
				return;
			}

			var statistic = await _categoryStatisticService.GetStatistic(user.Id, DateTime.Today.AddMonths(-3), DateTime.Today.AddMonths(-1));

			await botClient.SendTextMessageAsync(chatId, "Статистика по категориям за последние 3 месяца.\n\n" + FormatStatistic(statistic),
				parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

		}

		private string FormatStatistic(List<CategoryStatistic> statistic)
		{
			string output = "Категория | Месяц1 | Месяц2 | Месяц3";

			foreach (var s in statistic)
			{
				output = s.CategoryId + " | ";

				foreach (var month in s.Months)
				{
					output += month.Amount + " | ";
				}

				output += "\n";
			}

			return output;
		}
	}
}
