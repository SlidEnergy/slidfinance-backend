﻿using System.Collections.Generic;
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

			var connectionsResponse = await _saltedge.ConnectionsListAsync(saltedgeAccount.CustomerId);

			var list = new List<SaltedgeBankAccounts>();

			foreach (var connection in connectionsResponse.Data)
			{
				var accountsResponse = await _saltedge.AccountListAsync(connection.Id);

				list.Add(new SaltedgeBankAccounts() { Connection = connection, Accounts = accountsResponse.Data });
			}

			return list;
		}

		private async Task<IEnumerable<SeConnection>> GetUserConnections(string userId)
		{
			var saltedgeAccount = await _context.GetSaltedgeAccountByIdWithAccessCheck(userId);

			var connectionsResponse = await _saltedge.ConnectionsListAsync(saltedgeAccount.CustomerId);

			return connectionsResponse.Data;
		}

		private async Task<SeConnection> GetSeConnectionBySaltedgeBankAccountId(string userId, string saltedgeBankAccountId)
		{
			var connections = await GetUserConnections(userId);

			foreach (var connection in connections)
			{
				var accountsResponse = await _saltedge.AccountListAsync(connection.Id);

				foreach (var responseAccount in accountsResponse.Data)
				{
					if (responseAccount.Id == saltedgeBankAccountId)
						return connection;
				}
			}

			return null;
		}

		public async Task<string> Refresh(string userId, string saltedgeBankAccountId)
		{
			var connection = await GetSeConnectionBySaltedgeBankAccountId(userId, saltedgeBankAccountId);


			var session = new SaltEdgeNetCore.Models.ConnectSession.RefreshSession()
			{
				ConnectionId = connection.Id
			};

			var response = await _saltedge.SessionRefreshAsync(session);

			return response.ConnectUrl;
		}

		public async Task<int> Import(string userId)
		{
			var accounts = await _context.GetBankAccountListWithAccessCheckAsync(userId);
			accounts = accounts.Where(x => x.SaltedgeBankAccountId != null).ToList();

			if (accounts.Count() == 0)
				return 0;

			int count = 0;

			var connections = await GetUserConnections(userId);

			foreach (var connection in connections)
			{
				var accountsResponse = await _saltedge.AccountListAsync(connection.Id);

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
			var transactionsResponse = await _saltedge.TransactionsListAsync(connection.Id, saltedgeAccount.Id);

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
						BankCategory = GetBankCategory(t),
						MccId = existingMcc?.Id,
						Mcc = existingMcc,
						DateTime = t.MadeOn.Value,
						Description = await GetDescription(t),
						Amount = (float)t.Amount.Value
					};
					list.Add(transaction);
				}
			}

			return list;
		}

		private async Task<string> GetDescription(SaltEdgeTransaction transaction)
		{
			if (!string.IsNullOrEmpty(transaction.Extra.Additional))
			{
				var merchantDescription = await GetMerchant(transaction);

				if (!string.IsNullOrEmpty(merchantDescription))
					return merchantDescription;
			}

			return transaction.Description;
		}

		private string GetBankCategory(SaltEdgeTransaction transaction)
		{
			return transaction.Extra.Type ?? transaction.Category ?? "";
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
			if (!string.IsNullOrEmpty(saltEdgeTransaction.Extra.Additional))
			{
				Regex regex = new Regex(@".*МСС: (\d{4})");
				var match = regex.Match(saltEdgeTransaction.Extra.Additional);

				if (match.Success)
				{
					return new Mcc(match.Groups[1].Value);
				}
			}

			return null;
		}

		private async Task<string> GetMerchant(SaltEdgeTransaction saltEdgeTransaction)
		{
			if (!string.IsNullOrEmpty(saltEdgeTransaction?.Extra?.MerchantId))
			{
				var merchant = await _saltedge.MerchantShowAsync(saltEdgeTransaction.Extra.MerchantId);

				return merchant?.Names?.Where(x => x.Mode == "name").Select(x => x.Value).FirstOrDefault();
			}

			if (!string.IsNullOrEmpty(saltEdgeTransaction?.Extra?.Additional))
			{
				Regex regex = new Regex(@"МСС: \d{4}");
				return regex.Replace(saltEdgeTransaction.Extra.Additional, "");
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

					var merchantDescription = await GetMerchant(t);

					if (!String.IsNullOrEmpty(merchantDescription))
					{
						var merchant = new Merchant() { MccId = existingMcc.Id, Name = merchantDescription, CreatedById = userId, Created = DateTime.Now };
						await _merchantService.AddAsync(merchant);
					}
				}
			}
		}
	}
}
