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
	public class WhichCardToPayDialog: ComponentDialog
	{
		protected readonly ILogger Logger;
		private readonly IUsersService _usersService;
		private readonly ICashbackService _service;

		public WhichCardToPayDialog(IUsersService usersService, ICashbackService service, ILogger<WhichCardToPayDialog> logger)
		{
			_usersService = usersService;
			_service = service;
			Logger = logger;
			AddDialog(new TextPrompt(nameof(TextPrompt)));
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
				PromptSearchStringStepAsync,
				SearchStepAsync
			}));


			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> PromptSearchStringStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var promptMessage = MessageFactory.Text("Введите название категории, магазина или mcc код.", null, InputHints.ExpectingInput);
			return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
		}

		private async Task<DialogTurnResult> SearchStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			if (stepContext.Context.Activity.ChannelId == Microsoft.Bot.Connector.Channels.Telegram)
			{
				var chatId = Convert.ToInt64(stepContext.Context.Activity.From.Id);
				var searchString = (string)stepContext.Result;

				if (!string.IsNullOrEmpty(searchString))
				{
					var user = await _usersService.GetByTelegramChatIdAsync(chatId);

					if (user == null)
					{
						//TODO: заменить на диалог для неавторизованного пользователя
						await stepContext.Context.SendActivityAsync("Пользователь не авторизован. Войдите в систему, перейдите на вкладку \"Настройки\" и нажмите привязать аккаунт Телеграм.", null, InputHints.IgnoringInput, cancellationToken);
					}

					var result = await _service.WhichCardToPay(user.Id, searchString);

					var message = string.Join("\r\n", result);

					await stepContext.Context.SendActivityAsync(message, null, InputHints.IgnoringInput, cancellationToken);
				}
				else
				{
					//TODO: обработать неверный ввод
				}
			}
			else 
			{
				await stepContext.Context.SendActivityAsync("Ваш канал не поддерживается.", null, InputHints.IgnoringInput, cancellationToken);
			}

			return await stepContext.EndDialogAsync(null, cancellationToken);
		}
	}
}
