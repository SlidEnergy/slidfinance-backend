using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace SlidFinance.TelegramBot.Controllers
{
	[Route("api/update")]
	[ApiController]
	public class UpdateController : ControllerBase
	{
		private IUpdateService _updateService;

		public UpdateController(IUpdateService updateService)
		{
			_updateService = updateService;
		}

		[HttpGet]
		public async Task<OkResult> Get()
		{
			return Ok();
		}

		[HttpPost]
		public async Task<OkResult> Post([FromBody]Update update)
		{
			if(update != null)
				await _updateService.EchoAsync(update);

			return Ok();
		}
	}
}
