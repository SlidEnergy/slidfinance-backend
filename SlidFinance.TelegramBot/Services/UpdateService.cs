using Microsoft.Extensions.Logging;
using SlidFinance.TelegramBot.Models.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace SlidFinance.TelegramBot
{
	public class UpdateService : IUpdateService
	{
		private readonly IBotService _botService;
		private readonly ILogger<UpdateService> _logger;
		private CommandList _commandList;

		public UpdateService(IBotService botService, CommandList commandList, ILogger<UpdateService> logger)
		{
			_botService = botService;
			_logger = logger;
			_commandList = commandList;
		}

		public async Task EchoAsync(Update update)
		{
			if (update.Type != UpdateType.Message)
			{
				return;
			}

			var message = update.Message;

			_logger.LogInformation("Received Message from {0}", message.Chat.Id);

			if (message.Type == MessageType.Text)
			{
				foreach (var command in _commandList.Commands)
				{
					if (command.Contains(message))
					{
						await command.Execute(message, _botService.Client);
						break;
					}
				}
			}
		}
	}
}
