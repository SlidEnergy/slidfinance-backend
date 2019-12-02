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

			var statistic = await _categoryStatisticService.GetStatistic(user.Id, DateTime.Today.AddMonths(-4), DateTime.Today.AddMonths(-1));

			await botClient.SendTextMessageAsync(chatId, "*Статистика по категориям за последние 3 месяца.*\n\n" + FormatStatistic(statistic),
				parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

		}

		private string FormatStatistic(List<CategoryStatistic> statistic)
		{
			int categoryLength = 20;
			int amountLength = 8;

			string output = $"```\n{Col("Категория", categoryLength)}|{Col("Месяц1", amountLength)}|{Col("Месяц2", amountLength)}|{Col("Месяц1", amountLength)}\n";

			foreach (var s in statistic)
			{
				output += $"{Col(s.CategoryId.ToString(), categoryLength)}";

				foreach (var month in s.Months)
				{
					output += "|" + Col(month.Amount, amountLength);
				}

				output += "\n";
			}

			return output + "```";
		}

		private string Col(string text, int length)
		{
			return " " + text + generateSpaces(length - text.Length - 2) + " ";
		}

		private string Col(int amount, int length)
		{
			return " " + generateSpaces(length - amount.ToString().Length - 2) + amount.ToString() + " ";
		}

		private string Col(float amount, int length)
		{
			return " " + generateSpaces(length - amount.ToString().Length - 2) + amount.ToString() + " ";
		}

		private string generateSpaces(int length)
		{
			string str = "";
			for (int i = 0; i < length; i++)
				str += " ";

			return str;
		}
	}
}
