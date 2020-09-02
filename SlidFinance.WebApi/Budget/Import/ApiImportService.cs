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
		private readonly IAccountsService _accountsService;

		public ApiImportService(IMapper mapper, IImportService importService, IMccService mccService, IMerchantService merchantService, IAccountsService accountsService)
		{
			_mapper = mapper;
			_service = importService;
			_mccService = mccService;
			_merchantService = merchantService;
			_accountsService = accountsService;
		}

		public async Task<int> Import(string userId, PatchAccountDataBindingModel data)
		{
			if (string.IsNullOrEmpty(userId))
				return 0;

			if (data == null)
				return 0;

			if (data.Transactions != null && data.Transactions.Count > 0)
			{
				await _mccService.AddMccIfNotExistAsync(GetMccList(data.Transactions));	

				await AddMerchantsIfNotExist(userId, data.Transactions);
			}

			var transactions = data.Transactions == null ? null : _mapper.Map<Transaction[]>(data.Transactions);

			BankAccount account = null;

			if (data.AccountId.HasValue)
				account = await _accountsService.GetByIdWithAccessCheckAsync(userId, data.AccountId.Value);
			else
				account = await GetAccountByCode(userId, data.Code);

			var count = await _service.Import(userId, account.Id, data.Balance, transactions);

			return count;
		}

		private async Task<BankAccount> GetAccountByCode(string userId, string accountCode)
		{
			var accounts = await _accountsService.GetListWithAccessCheckAsync(userId);

			var account = accounts.FirstOrDefault(x => x.Code == accountCode);

			if (account == null)
				throw new EntityNotFoundException();

			return account;
		}

		public List<Mcc> GetMccList(ICollection<Dto.ImportTransaction> transactions)
		{
			return transactions.Where(t => t.Mcc != null).Select(t => new Mcc(t.Mcc.Value.ToString("D4"))).ToList();
		}

		private async Task AddMerchantsIfNotExist(string userId, ICollection<Dto.ImportTransaction> transactions)
		{
			var mccList = await _mccService.GetListAsync();

			foreach (var t in transactions)
			{
				if (t.Mcc.HasValue)
				{
					var mcc = mccList.FirstOrDefault(x => x.Code == t.Mcc.Value.ToString("D4"));

					if (mcc == null)
						throw new Exception("МСС код не найден");

					var merchant = new Merchant() { MccId = mcc.Id, Name = t.Description, CreatedById = userId, Created = DateTime.Now };
					await _merchantService.AddAsync(merchant);
				}
			}
		}
	}
}
