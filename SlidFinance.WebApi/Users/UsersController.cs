using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.Domain;
using SlidFinance.WebApi.Dto;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	[Route("api/v1/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly IUsersService _usersService;
		private readonly ITokenService _tokenService;

		public UsersController(IMapper mapper, IUsersService usersService, ITokenService tokenService)
		{
			_mapper = mapper;
			_usersService = usersService;
			_tokenService = tokenService;
		}

		[HttpGet]
		[ProducesResponseType(200)]
		[Authorize(Policy = Policy.MustBeAdmin)]
		public async Task<ActionResult<IEnumerable<Dto.User>>> GetList()
		{
			var users = await _usersService.GetListAsync();

			return _mapper.Map<Dto.User[]>(users);
		}

		[HttpGet("current")]
		[ProducesResponseType(200)]
		[ProducesResponseType(404)]
		[Authorize]
		public async Task<ActionResult<Dto.User>> GetCurrentUser()
		{
			var userId = User.GetUserId();
			
			var user = await _usersService.GetById(userId);

			if (user == null)
			{
				return NotFound();
			}

			bool isAdmin = await _usersService.IsAdmin(user);

			return _mapper.Map<Dto.User>(user, opt => opt.AfterMap((src, dest) => ((Dto.User)dest).IsAdmin = isAdmin));
		}

		[HttpPost("register")]
		public async Task<ActionResult<Dto.User>> Register(RegisterBindingModel model)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = _mapper.Map<ApplicationUser>(model);

			var result = await _usersService.CreateAccount(user, model.Password);

			if (!result.Succeeded) {
				foreach (var e in result.Errors)
				{
					ModelState.TryAddModelError(e.Code, e.Description);
				}

				return BadRequest(ModelState);
			}

			return Created("", _mapper.Map<Dto.User>(user));
		}

		[HttpPost("token")]
		[ProducesResponseType(200)]
		public async Task<ActionResult<TokenInfo>> GetToken(LoginBindingModel userData)
		{
			if (!ModelState.IsValid)
				return BadRequest();

			try
			{
				var tokens = await _tokenService.CheckCredentialsAndGetToken(userData.Email, userData.Password);

				return new TokenInfo() { Token = tokens.Token, RefreshToken = tokens.RefreshToken, Email = userData.Email };
			}
			catch(AuthenticationException)
			{
				return BadRequest();
			}
		}
	}
}
