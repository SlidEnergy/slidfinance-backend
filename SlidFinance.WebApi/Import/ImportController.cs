using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;

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
		[HttpPost("token")]
		[ProducesResponseType(200)]
		public async Task<ActionResult<TokensCortage>> GetToken()
		{
			var userId = User.GetUserId();

			var user = await _usersService.GetById(userId);

			if (user == null)
			{
				return NotFound();
			}

			return await _tokenService.GenerateAccessAndRefreshTokens(user, AccessMode.Import);
		}
	}
}
