using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.Controllers
{
	[Route("api/[controller]")]
	[Authorize(Policy = Policy.MustBeAllAccessMode)]
	[ApiController]
    public class TelegramController : ControllerBase
    {
		private readonly ITelegramService _service;

		public TelegramController(ITelegramService service)
		{
			_service = service;
		}

		[HttpPost]
		[ProducesResponseType(200)]
		public async Task<ActionResult<IEnumerable<Dto.BankAccount>>> Post(TelegramUser user)
		{
			var userId = User.GetUserId();

			try
			{
				await _service.ConnectTelegramUser(userId, user);
			}
			catch (ArgumentException exc)
			{
				return BadRequest();
			}

			return Ok();
		}
	}
}