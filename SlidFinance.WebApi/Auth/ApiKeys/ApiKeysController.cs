using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{

	[Route("api/v1/[controller]")]
	[ApiController]
	public class ApiKeysController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IApiImportService _service;
		private readonly ITokenService _tokenService;
		private readonly IUsersService _usersService;

		public ApiKeysController(IMapper mapper, IApiImportService importService, ITokenService tokenService, IUsersService usersService)
		{
			_mapper = mapper;
			_service = importService;
			_tokenService = tokenService;
			_usersService = usersService;
		}

		[Authorize(Policy = Policy.MustBeAllAccessMode)]
		[HttpPost()]
		[ProducesResponseType(200)]
		public async Task<ActionResult<string>> GetApiKey()
		{
			var userId = User.GetUserId();

			var user = await _usersService.GetById(userId);

			if (user == null)
				throw new AuthenticationException();

			return await _tokenService.GenerateToken(user, AuthTokenType.ApiKey);
		}
	}
}
