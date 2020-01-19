using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
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

		public MainDialog(WhichCardToPayDialog wichCardToPayDialog, CategoryStatisticDialog categoryStatisticDialog, ILogger<WhichCardToPayDialog> logger)
		{
			Logger = logger;
			AddDialog(new TextPrompt(nameof(TextPrompt)));
			AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
			AddDialog(wichCardToPayDialog);
			AddDialog(categoryStatisticDialog);
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[] {
				PromptCommandStepAsync,
				ExecuteCommandStepAsync,
				RepeatStepAsync
			}));

			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> PromptCommandStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var menu = new List<string> { "Какой картой платить?", "Статистика по категориям" };

			var promptMessage = MessageFactory.Text("Выберите действие", null, InputHints.ExpectingInput);
			return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions {
				Prompt = promptMessage,
				Choices = ChoiceFactory.ToChoices(menu),
				RetryPrompt = MessageFactory.Text("Выберите из списка"),
				Style = ListStyle.SuggestedAction
			}, cancellationToken);
		}

		private async Task<DialogTurnResult> ExecuteCommandStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var choice = (FoundChoice)stepContext.Result;

			switch (choice.Index)
			{ 
				case 0:
					return await stepContext.BeginDialogAsync(nameof(WhichCardToPayDialog), new PromptOptions(), cancellationToken);

				case 1:
					return await stepContext.BeginDialogAsync(nameof(CategoryStatisticDialog), new PromptOptions(), cancellationToken);

				default:
					await stepContext.Context.SendActivityAsync("Комманда не распознана", null, InputHints.IgnoringInput, cancellationToken);
					break;
			}

			return await stepContext.NextAsync(null, cancellationToken);
		}

		private async Task<DialogTurnResult> RepeatStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			return await stepContext.ReplaceDialogAsync(nameof(MainDialog), null, cancellationToken);
		}
	}
}
