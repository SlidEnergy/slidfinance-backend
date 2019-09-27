using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
    
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private IImportService _service;
		private ITokenService _tokenService;
		private IUsersService _usersService;

		public ImportController(IMapper mapper, IImportService importService, ITokenService tokenService, IUsersService usersService)
        {
            _mapper = mapper;
            _service = importService;
			_tokenService = tokenService;
			_usersService = usersService;
		}

		[Authorize(Policy = Policy.MustBeImportAccessMode)]
		[HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<int>> Import(PatchAccountDataBindingModel data)
        {
            var userId = User.GetUserId();

            var count = await _service.Import(userId, data.Code, data.Balance, _mapper.Map<Transaction[]>(data.Transactions));

            return count;
        }

		[Authorize(Policy = Policy.MustBeAllAccessMode)]
		[HttpPost("token")]
		[ProducesResponseType(200)]
		[ProducesResponseType(401)]
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
