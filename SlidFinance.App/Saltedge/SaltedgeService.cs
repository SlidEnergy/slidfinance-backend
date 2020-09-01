using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaltEdgeNetCore.Client;
using SaltEdgeNetCore.Models.Account;
using SaltEdgeNetCore.Models.Connections;
using SaltEdgeNetCore.Models.Transaction;
using SlidFinance.Domain;
using System.Linq;

namespace SlidFinance.App.Saltedge
{
	public class SaltedgeService : ISaltedgeService
	{
		private IApplicationDbContext _context;
		private readonly ISaltEdgeClientV5 _saltedge;
		private readonly IImportService _importService;

		public SaltedgeService(IApplicationDbContext context, ISaltEdgeClientV5 saltedge, IImportService importService)
		{
			_context = context;
			_saltedge = saltedge;
			_importService = importService;
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

			foreach(var connection in connectionsResponse.Data)
			{
				var accountsResponse = _saltedge.AccountList(connection.Id);

				list.Add(new SaltedgeBankAccounts() { Connection = connection, Accounts = accountsResponse.Data });
			}

			return list;
		}

		public async Task Import(string userId)
		{
			var saltedgeAccount = await _context.GetSaltedgeAccountByIdWithAccessCheck(userId);

			var connectionsResponse = _saltedge.ConnectionsList(saltedgeAccount.CustomerId);

			var accounts = await _context.GetAccountListWithAccessCheckAsync(userId);
			accounts = accounts.Where(x => x.SaltedgeBankAccountId != null).ToList();

			if (accounts.Count() == 0)
				return;

			foreach (var connection in connectionsResponse.Data)
			{
				var accountsResponse = _saltedge.AccountList(connection.Id);

				foreach (var responseAccount in accountsResponse.Data)
				{
					var account = accounts.Where(x => x.SaltedgeBankAccountId == responseAccount.Id).FirstOrDefault();

					if (account == null)
						continue;

					await ImportByAccount(userId, connection, responseAccount, account);
				}
			}
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
				//await AddMccIfNotExist(data.Transactions);

				//await AddMerchantsIfNotExist(userId, data.Transactions);
			}

			//var transactions = saltedgeTransactions == null ? null : _mapper.Map<Transaction[]>(saltedgeTransactions);

			//var count = await _importService.Import(userId, account, null, transactions);

			//return count;

			return 0;
		}
	}
}
