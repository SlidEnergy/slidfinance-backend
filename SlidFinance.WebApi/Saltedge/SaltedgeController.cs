using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.Controllers
{
	[Route("saltedge")]
	[ApiController]
	public class SaltedgeController : ControllerBase
	{
		public SaltedgeController()
		{
		}

		[HttpPost("customer")]
		public async Task<ActionResult> AddCustomer(string customerId)
		{
			var userId = User.GetUserId();



			return Ok();
		}
	}
}