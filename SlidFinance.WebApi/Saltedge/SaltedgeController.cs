using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SaltEdgeNetCore.Models.Account;
using SaltEdgeNetCore.Models.Connections;
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
		private readonly IHostingEnvironment _hostingEnv;

		public SaltedgeController(ISaltedgeService saltedgeService, IHostingEnvironment hostingEnv)
		{
			_saltedgeService = saltedgeService;
			_hostingEnv = hostingEnv;
		}

		[HttpPost()]
		public async Task<ActionResult<SaltedgeAccount>> AddCustomer(SaltedgeAccount saltedgeAccount)
		{
			var userId = User.GetUserId();

			var created = await _saltedgeService.AddSaltedgeAccount(userId, saltedgeAccount);

			return Created("", created);
		}

		[HttpPost("import/run")]
		public async Task<ActionResult<int>> RunImport()
		{
			var userId = User.GetUserId();

			var count = await _saltedgeService.Import(userId);

			return Ok(count);
		}

		[HttpPost("refresh/all")]
		public async Task<ActionResult<int>> RefreshAll()
		{
			var userId = User.GetUserId();

			return Ok(0);
		}

		[HttpGet("accounts")]
		public async Task<ActionResult<IEnumerable<SaltedgeBankAccounts>>> GetList()
		{
			var userId = User.GetUserId();

			if (_hostingEnv.IsDevelopment())
			{
				return Ok(GenerateTestSaltedgeBankAccounts());
			}

			var list = await _saltedgeService.GetSaltedgeBankAccounts(userId);

			return Ok(list);
		}


		private IEnumerable<SaltedgeBankAccounts> GenerateTestSaltedgeBankAccounts()
		{
			return new List<SaltedgeBankAccounts>()
				{
					new SaltedgeBankAccounts()
					{
						Connection = new SeConnection()
						{
							Id = "1",
							ProviderName = "Provider #1"
						},
						Accounts = new List<SeAccount>()
						{
							new SeAccount()
							{
								Id = "1",
								Name = "Account #1",
								Balance = 100
							},
							new SeAccount()
							{
								Id = "2",
								Name = "Account #2",
								Balance = 200
							},

						}
					},
					new SaltedgeBankAccounts()
					{
						Connection = new SeConnection()
						{
							Id = "2",
							ProviderName = "Provider #2"
						},
						Accounts = new List<SeAccount>()
						{
							new SeAccount()
							{
								Id = "3",
								Name = "Account #3",
								Balance = 300
							},
							new SeAccount()
							{
								Id = "4",
								Name = "Account #4",
								Balance = 400
							},

						}
					}
				};
		}
	}
}