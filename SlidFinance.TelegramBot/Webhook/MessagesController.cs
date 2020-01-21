using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using System.Net.Http;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace SlidFinance.TelegramBot.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MessagesController : ControllerBase
	{
		private readonly IBotFrameworkHttpAdapter Adapter;
		private readonly IBot Bot;

		public MessagesController(IBotFrameworkHttpAdapter adapter, IBot bot)
		{
			Adapter = adapter;
			Bot = bot;
		}

		[HttpPost, HttpGet]
		public async Task PostAsync()
		{
			// Delegate the processing of the HTTP POST to the adapter.
			// The adapter will invoke the bot.
			await Adapter.ProcessAsync(Request, Response, Bot);
		}
	}
}
