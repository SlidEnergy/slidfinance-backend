using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.App.Saltedge;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.Controllers
{
	[Authorize(Policy = Policy.MustBeAllAccessMode)]
	[Route("api/v1/[controller]")]
	[ApiController]
	public class SaltedgeController : ControllerBase
	{
		private readonly ISaltedgeService _saltedgeService;

		public SaltedgeController(ISaltedgeService saltedgeService)
		{
			_saltedgeService = saltedgeService;
		}

		[HttpPost()]
		public async Task<ActionResult<SaltedgeAccount>> AddCustomer(SaltedgeAccount saltedgeAccount)
		{
			var userId = User.GetUserId();

			var created = await _saltedgeService.AddSaltedgeAccount(userId, saltedgeAccount);

			return Created("", created);
		}

		[HttpPost("import/run")]
		public async Task<ActionResult> RunImport()
		{
			var userId = User.GetUserId();

			await _saltedgeService.Import(userId);

			return Ok();
		}

		[HttpGet("accounts")]
		public async Task<ActionResult<IEnumerable<SaltedgeBankAccounts>>> GetList()
		{
			var userId = User.GetUserId();

			var list = await _saltedgeService.GetSaltedgeBankAccounts(userId);

			return Ok(list);
		}
	}
}