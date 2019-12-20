using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlidFinance.App;
using SlidFinance.Domain;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace SlidFinance.WebApi
{
    
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImportController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IImportService _service;
		private readonly ITokenService _tokenService;
		private readonly IUsersService _usersService;
		private readonly IMccService _mccService;

		public ImportController(IMapper mapper, IImportService importService, ITokenService tokenService, IUsersService usersService, IMccService mccService)
        {
            _mapper = mapper;
            _service = importService;
			_tokenService = tokenService;
			_usersService = usersService;
			_mccService = mccService;
		}

		[Authorize(Policy = Policy.MustBeAllOrImportAccessMode)]
		[HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<int>> Import(PatchAccountDataBindingModel data)
        {
            var userId = User.GetUserId();

			var mccList = await _mccService.GetListAsync();

			foreach (var t in data.Transactions)
			{
				if (t.Mcc.HasValue)
				{
					var mcc = mccList.FirstOrDefault(x => x.Code == t.Mcc.Value.ToString("D4"));
					if (mcc == null)
					{
						mcc = new Mcc() { Code = t.Mcc.Value.ToString("D4"), IsSystem = false };
						await _mccService.AddAsync(mcc);
					}
				}
			}

            var count = await _service.Import(userId, data.Code, data.Balance, _mapper.Map<Transaction[]>(data.Transactions));

            return count;
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
