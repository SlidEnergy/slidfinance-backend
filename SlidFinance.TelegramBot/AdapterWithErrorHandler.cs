using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SlidFinance.TelegramBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.TelegramBot
{
	public class AdapterWithErrorHandler : BotFrameworkHttpAdapter
	{
		public AdapterWithErrorHandler(ICredentialProvider credentialProvider, ILogger<BotFrameworkHttpAdapter> logger)
			: base(credentialProvider, null, logger)
		{
			OnTurnError = async (turnContext, exception) =>
			{
				// Log any leaked exception from the application.
				logger.LogError(exception, $"[OnTurnError] unhandled error : {exception.Message}");

				// Send a message to the user
				await turnContext.SendActivityAsync("The bot encounted an error or bug.");
				await turnContext.SendActivityAsync("To continue to run this bot, please fix the bot source code.");

				// Send a trace activity, which will be displayed in the Bot Framework Emulator
				await turnContext.TraceActivityAsync("OnTurnError Trace", exception.Message, "https://www.botframework.com/schemas/error", "TurnError");
			};
		}
	}
}
