using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.Controllers
{
	[Route("saltedge/account")]
	[ApiController]
    public class SaltedgeAccountInformationController : ControllerBase
    {
		public SaltedgeAccountInformationController()
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

		[HttpPost("destroy")]
		public async Task<ActionResult> Destroy()
		{
			return Ok();
		}

		[HttpPost("notify")]
		public async Task<ActionResult> Notify()
		{
			return Ok();
		}

		[HttpPost("service")]
		public async Task<ActionResult> Service()
		{
			return Ok();
		}
	}
}