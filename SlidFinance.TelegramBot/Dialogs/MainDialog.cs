using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlidFinance.TelegramBot.Dialogs
{
	public class MainDialog: ComponentDialog
	{
		protected readonly ILogger Logger;

		public MainDialog(WhichCardToPayDialog wichCardToPayDialog, ILogger<WhichCardToPayDialog> logger)
		{
			Logger = logger;
			AddDialog(new TextPrompt(nameof(TextPrompt)));
			AddDialog(wichCardToPayDialog);
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
				PromptCommandStepAsync,
				ExecuteCommandStepAsync
			}));

			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> PromptCommandStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var promptMessage = MessageFactory.Text("Введите команду", null, InputHints.ExpectingInput);
			return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
		}

		private async Task<DialogTurnResult> ExecuteCommandStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var command = (string)stepContext.Result;

			switch (command)
			{ 
				case "/whichcardtopay":
					return await stepContext.BeginDialogAsync(nameof(WhichCardToPayDialog), new PromptOptions(), cancellationToken);

				default:
					await stepContext.Context.SendActivityAsync("Комманда не распознана", null, InputHints.IgnoringInput, cancellationToken);
					break;
			}

			return await stepContext.EndDialogAsync(null, cancellationToken);
		}
	}
}
