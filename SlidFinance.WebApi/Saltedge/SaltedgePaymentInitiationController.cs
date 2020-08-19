using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.Controllers
{
	[Route("saltedge/payment")]
	[ApiController]
    public class SaltedgePaymentInitiationController : ControllerBase
    {
		public SaltedgePaymentInitiationController()
		{
		}

		[HttpPost("success")]
		public async Task<ActionResult> Success()
		{
			return Ok();
		}

		[HttpPost("fail")]
		public async Task<ActionResult> Fail()
		{
			return Ok();
		}

		[HttpPost("notify")]
		public async Task<ActionResult> Notify()
		{
			return Ok();
		}

		[HttpPost("interactive")]
		public async Task<ActionResult> Interactive()
		{
			return Ok();
		}
	}
}