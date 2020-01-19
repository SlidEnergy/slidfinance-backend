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
	public class DialogAndWelcomeBot<T> : DialogBot<T>
		where T : Dialog
	{
		public DialogAndWelcomeBot(ConversationState conversationState, UserState userState, T dialog, DialogList dialogList, ILogger<DialogBot<T>> logger)
			: base(conversationState, userState, dialog, dialogList, logger)
		{
		}

		protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
		{
			foreach (var member in membersAdded)
			{
				// Greet anyone that was not the target (recipient) of this message.
				// To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
				if (member.Id != turnContext.Activity.Recipient.Id)
				{
					var activity = MessageFactory.Text("Добро пожаловать!");
					await turnContext.SendActivityAsync(activity, cancellationToken);
					await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
				}
			}
		}
	}
}
