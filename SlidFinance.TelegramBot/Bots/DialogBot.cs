using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using SlidFinance.TelegramBot.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SlidFinance.TelegramBot.Bots
{
	public class DialogBot<T> : ActivityHandler
			where T : Dialog
	{
		protected readonly Dialog Dialog;
		protected readonly BotState ConversationState;
		protected readonly BotState UserState;
		protected readonly ILogger Logger;
		protected readonly DialogList _dialogList;

		public DialogBot(ConversationState conversationState, UserState userState, T dialog, DialogList dialogList, ILogger<DialogBot<T>> logger)
		{
			ConversationState = conversationState;
			UserState = userState;
			Dialog = dialog;
			Logger = logger;
			_dialogList = dialogList;
		}

		public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
		{
			await base.OnTurnAsync(turnContext, cancellationToken);

			// Save any state changes that might have occured during the turn.
			await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
			await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
		}

		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
		{
			Logger.LogInformation("Running dialog with Message Activity.");

			// Run the Dialog with the new message Activity.
			await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
		}
	}
}
