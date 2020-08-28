using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaltEdgeNetCore.Models.Connections;
using SlidFinance.App;
using SlidFinance.App.Saltedge;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.WebApi.Controllers
{
	[Route("saltedge/connections")]
	[ApiController]
	public class SaltedgeConnectionsController : ControllerBase
	{
		private readonly ISaltedgeService _saltedgeService;

		public SaltedgeConnectionsController(ISaltedgeService saltedgeService)
		{
			_saltedgeService = saltedgeService;
		}

		[HttpGet()]
		public async Task<ActionResult<IEnumerable<SeConnection>>> GetList()
		{
			var userId = User.GetUserId();

			var list = await _saltedgeService.GetConnections(userId);

			return Ok(list);
		}
	}
}