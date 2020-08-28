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
	}
}