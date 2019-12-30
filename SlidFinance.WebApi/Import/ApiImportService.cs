using AutoMapper;
using SlidFinance.App;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	public class ApiImportService : IApiImportService
	{
		private readonly IMapper _mapper;
		private readonly IImportService _service;
		private readonly IMccService _mccService;
		private readonly IMerchantService _merchantService;

		public ApiImportService(IMapper mapper, IImportService importService, IMccService mccService, IMerchantService merchantService)
		{
			_mapper = mapper;
			_service = importService;
			_mccService = mccService;
			_merchantService = merchantService;
		}

		public async Task<int> Import(string userId, PatchAccountDataBindingModel data)
		{
			await AddMccIfNotExist(data.Transactions);

			await AddMerchantsIfNotExist(userId, data.Transactions);

			var transactions = _mapper.Map<Transaction[]>(data.Transactions);

			var count = await _service.Import(userId, data.Code, data.Balance, transactions);

			return count;
		}

		private async Task AddMerchantsIfNotExist(string userId, ICollection<Dto.ImportTransaction> transactions)
		{
			var merchantList = await _merchantService.GetListAsync();

			foreach (var t in transactions)
			{
				if (t.Mcc.HasValue)
				{
					var merchant = new Models.Merchant() { MccId = t.Mcc.Value, Name = t.Description, CreatedById = userId, Created = DateTime.Now };
					await _merchantService.AddAsync(merchant);
				}
			}
		}

		private async Task AddMccIfNotExist(ICollection<Dto.ImportTransaction> transactions)
		{
			var mccList = await _mccService.GetListAsync();

			foreach (var t in transactions)
			{
				if (t.Mcc.HasValue)
				{
					var mcc = mccList.FirstOrDefault(x => x.Code == t.Mcc.Value.ToString("D4"));
					if (mcc == null)
					{
						mcc = new Mcc() { Code = t.Mcc.Value.ToString("D4"), IsSystem = false, Title = "", Category = MccCategory.None };
						await _mccService.AddAsync(mcc);
					}
				}
			}
		}
	}
}
