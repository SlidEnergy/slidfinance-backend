using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaltEdgeNetCore.Client;
using SaltEdgeNetCore.Models.Account;
using SaltEdgeNetCore.Models.Connections;
using SaltEdgeNetCore.Models.Transaction;
using SlidFinance.Domain;
using System.Linq;
using System.Text.RegularExpressions;
using System;

namespace SlidFinance.App.Saltedge
{
	public class SaltedgeService : ISaltedgeService
	{
		private IApplicationDbContext _context;
		private readonly ISaltEdgeClientV5 _saltedge;
		private readonly IImportService _importService;
		private readonly IMccService _mccService;
		private readonly IMerchantService _merchantService;

		public SaltedgeService(IApplicationDbContext context, ISaltEdgeClientV5 saltedge, IImportService importService, IMccService mccService, IMerchantService merchantService)
		{
			_context = context;
			_saltedge = saltedge;
			_importService = importService;
			_mccService = mccService;
			_merchantService = merchantService;
		}

		public async Task<SaltedgeAccount> AddSaltedgeAccount(string userId, SaltedgeAccount saltedgeAccount)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

			_context.SaltedgeAccounts.Add(saltedgeAccount);
			await _context.SaveChangesAsync();

			_context.TrusteeSaltedgeAccounts.Add(new TrusteeSaltedgeAccount(user, saltedgeAccount));
			await _context.SaveChangesAsync();

			return saltedgeAccount;
		}

		public async Task<IEnumerable<SaltedgeBankAccounts>> GetSaltedgeBankAccounts(string userId)
		{
			var saltedgeAccount = await _context.GetSaltedgeAccountByIdWithAccessCheck(userId);

			var connectionsResponse = _saltedge.ConnectionsList(saltedgeAccount.CustomerId);

			var list = new List<SaltedgeBankAccounts>();

			foreach (var connection in connectionsResponse.Data)
			{
				var accountsResponse = _saltedge.AccountList(connection.Id);

				list.Add(new SaltedgeBankAccounts() { Connection = connection, Accounts = accountsResponse.Data });
			}

			return list;
		}

		public async Task<int> Import(string userId)
		{
			var saltedgeAccount = await _context.GetSaltedgeAccountByIdWithAccessCheck(userId);

			var connectionsResponse = _saltedge.ConnectionsList(saltedgeAccount.CustomerId);

			var accounts = await _context.GetAccountListWithAccessCheckAsync(userId);
			accounts = accounts.Where(x => x.SaltedgeBankAccountId != null).ToList();

			if (accounts.Count() == 0)
				return 0;

			int count = 0;

			foreach (var connection in connectionsResponse.Data)
			{
				var accountsResponse = _saltedge.AccountList(connection.Id);

				foreach (var responseAccount in accountsResponse.Data)
				{
					var account = accounts.Where(x => x.SaltedgeBankAccountId == responseAccount.Id).FirstOrDefault();

					if (account == null)
						continue;

					count += await ImportByAccount(userId, connection, responseAccount, account);
				}
			}

			return count;
		}

		private async Task<int> ImportByAccount(string userId, SeConnection connection, SeAccount saltedgeAccount, BankAccount account)
		{
			var transactionsResponse = _saltedge.TransactionsList(connection.Id, saltedgeAccount.Id);

			return await ImportTransactions(userId, account, transactionsResponse.Data);
		}

		private async Task<int> ImportTransactions(string userId, BankAccount account, IEnumerable<SaltEdgeTransaction> saltedgeTransactions)
		{
			if (string.IsNullOrEmpty(userId))
				return 0;

			if (saltedgeTransactions != null && saltedgeTransactions.Count() > 0)
			{
				await _mccService.AddMccIfNotExistAsync(GetMccList(saltedgeTransactions));

				await AddMerchantsIfNotExist(userId, saltedgeTransactions);
			}

			var transactions = await ConvertToTransactions(saltedgeTransactions);

			var balance = saltedgeTransactions.Last().Extra.AccountBalanceSnapshot;

			var count = await _importService.Import(userId, account.Id, (float?)balance, transactions.ToArray());

			return count;
		}

		private async Task<ICollection<Transaction>> ConvertToTransactions(IEnumerable<SaltEdgeTransaction> saltEdgeTransactions)
		{
			List<Transaction> list = new List<Transaction>(saltEdgeTransactions.Count());

			var mccList = await _mccService.GetListAsync();

			foreach (var t in saltEdgeTransactions)
			{
				var mcc = GetMcc(t);
				var existingMcc = mcc == null ? null : mccList.FirstOrDefault(x => x.Code == mcc.Code);

				if (t.MadeOn.HasValue && t.Amount.HasValue)
				{
					var transaction = new Transaction()
					{
						BankCategory = t.Category ?? "",
						MccId = existingMcc?.Id,
						Mcc = existingMcc,
						DateTime = t.MadeOn.Value,
						Description = t.Description,
						Amount = (float)t.Amount.Value
					};
					list.Add(transaction);
				}
			}

			return list;
		}

		private ICollection<Mcc> GetMccList(IEnumerable<SaltEdgeTransaction> saltEdgeTransactions)
		{
			List<Mcc> list = new List<Mcc>();

			foreach (var t in saltEdgeTransactions)
			{
				var mcc = GetMcc(t);
				if (mcc != null)
					list.Add(mcc);
			}

			return list;
		}

		private Mcc GetMcc(SaltEdgeTransaction saltEdgeTransaction)
		{
			if (string.IsNullOrEmpty(saltEdgeTransaction.Extra.Additional))
			{
				Regex regex = new Regex(@".*MCC: (\d{4})");
				var match = regex.Match(saltEdgeTransaction.Extra.Additional);

				if (match.Success)
				{
					return new Mcc(match.Groups[0].Value);
				}
			}

			return null;
		}

		private string GetMerchantDescription(SaltEdgeTransaction saltEdgeTransaction)
		{
			if (string.IsNullOrEmpty(saltEdgeTransaction.Extra.Additional))
			{
				Regex regex = new Regex(@"(.*)MCC: \d{4}");
				var match = regex.Match(saltEdgeTransaction.Extra.Additional);

				if (match.Success)
					return match.Groups[0].Value;
			}

			return null;
		}

		private async Task AddMerchantsIfNotExist(string userId, IEnumerable<SaltEdgeTransaction> saltEdgeTransactions)
		{
			var mccList = await _mccService.GetListAsync();

			foreach (var t in saltEdgeTransactions)
			{
				var mcc = GetMcc(t);
				if (mcc != null)
				{
					var existingMcc = mccList.FirstOrDefault(x => x.Code == mcc.Code);

					if (existingMcc == null)
						throw new Exception("МСС код не найден");

					var merchantDescription = GetMerchantDescription(t);

					if (String.IsNullOrEmpty(merchantDescription))
					{
						var merchant = new Merchant() { MccId = existingMcc.Id, Name = merchantDescription, CreatedById = userId, Created = DateTime.Now };
						await _merchantService.AddAsync(merchant);
					}
				}
			}
		}
	}
}
