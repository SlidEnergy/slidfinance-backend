using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SlidFinance.App;
using SlidFinance.Domain;
using SlidFinance.WebApi.Dto;
using System;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{

	[Route("api/v1/[controller]")]
	[ApiController]
	public class ImportController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IApiImportService _service;
		private readonly ITokenService _tokenService;
		private readonly IUsersService _usersService;

		public ImportController(IMapper mapper, IApiImportService importService, ITokenService tokenService, IUsersService usersService)
		{
			_mapper = mapper;
			_service = importService;
			_tokenService = tokenService;
			_usersService = usersService;
		}

		[Authorize(Policy = Policy.MustBeAllOrImportAccessMode)]
		[HttpPost]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		public async Task<ActionResult<int>> Import(PatchAccountDataBindingModel data)
		{
			var userId = User.GetUserId();

			return await _service.Import(userId, data);
		}

		[Authorize(Policy = Policy.MustBeAllAccessMode)]
		[HttpPost("refreshToken")]
		[ProducesResponseType(200)]
		public async Task<ActionResult<string>> GetRefreshToken()
		{
			var userId = User.GetUserId();

			var user = await _usersService.GetById(userId);

			if (user == null)
				throw new AuthenticationException();

			return await _tokenService.GenerateToken(user, AuthTokenType.ImportToken);
		}

		[HttpPost("token")]
		public async Task<ActionResult<TokenInfo>> Refresh(TokensCortage tokens)
		{
			try
			{
				var newTokens = await _tokenService.RefreshImportToken(tokens.RefreshToken);

				return new TokenInfo() { Token = newTokens.Token, RefreshToken = newTokens.RefreshToken };
			}
			catch (SecurityTokenException)
			{
				return BadRequest();
			}
		}
	}
}
