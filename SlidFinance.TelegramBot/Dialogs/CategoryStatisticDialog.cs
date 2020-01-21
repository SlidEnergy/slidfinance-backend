using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using SlidFinance.App;
using SlidFinance.App.Analysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlidFinance.TelegramBot.Dialogs
{
	public class CategoryStatisticDialog: ComponentDialog
	{
		protected readonly ILogger Logger;
		private readonly IUsersService _usersService;
		private readonly ICategoryStatisticService _service;

		public CategoryStatisticDialog(IUsersService usersService, ICategoryStatisticService service, ILogger<WhichCardToPayDialog> logger)
		{
			_usersService = usersService;
			_service = service;
			Logger = logger;
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
				ShowStatisticStepAsync,
			}));


			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> ShowStatisticStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			if (stepContext.Context.Activity.ChannelId == Microsoft.Bot.Connector.Channels.Telegram)
			{
				var chatId = Convert.ToInt64(stepContext.Context.Activity.From.Id);
					var user = await _usersService.GetByTelegramChatIdAsync(chatId);

					if (user == null)
					{
						//TODO: заменить на диалог для неавторизованного пользователя
						await stepContext.Context.SendActivityAsync("Пользователь не авторизован. Войдите в систему, перейдите на вкладку \"Настройки\" и нажмите привязать аккаунт Телеграм.", null, InputHints.IgnoringInput, cancellationToken);
						return await stepContext.EndDialogAsync(null, cancellationToken);	
					}

					var statistic = await _service.GetStatistic(user.Id, DateTime.Today.AddMonths(-4), DateTime.Today.AddMonths(-1));

					var message = "Статистика по категориям за последние 3 месяца.\n\n" + FormatStatistic(statistic);

					await stepContext.Context.SendActivityAsync(message, null, InputHints.IgnoringInput, cancellationToken);
			}
			else 
			{
				await stepContext.Context.SendActivityAsync("Ваш канал не поддерживается.", null, InputHints.IgnoringInput, cancellationToken);
			}

			return await stepContext.EndDialogAsync(null, cancellationToken);
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
