using Microsoft.EntityFrameworkCore;
using SlidFinance.App.Utils;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
    public class TransactionsService : ITransactionsService
	{
        private DataAccessLayer _dal;
		private IApplicationDbContext _context;

		public TransactionsService(DataAccessLayer dal, IApplicationDbContext context)
        {
            _dal = dal;
			_context = context;
		}

        public Task<Transaction> GetById(string userId, int id) => GetByIdWithChecks(userId, id);

        public async Task<List<Transaction>> GetListWithAccessCheckAsync(string userId, int? accountId = null, int? categoryId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
			if (accountId.HasValue)
				ArgumentValidator.ValidateId(accountId.Value);

			if (categoryId.HasValue)
				ArgumentValidator.ValidateId(categoryId.Value);

			if(startDate.HasValue && endDate.HasValue)
				ArgumentValidator.ValidatePeriodAllowEqual(startDate.Value, endDate.Value);

			var user = await _context.Users.FindAsync(userId);

			var transactions = await _context.TrusteeAccounts
				.Where(x => x.TrusteeId == user.TrusteeId)
				.Join(_context.Transactions, a => a.AccountId, t => t.AccountId, (a, t) => t)
				.ToListAsync();

			if (accountId != null)
				transactions = transactions.Where(t => t.AccountId == accountId).ToList();

			if (categoryId != null) 
				transactions = transactions.Where(t => t.CategoryId == categoryId).ToList();

			if(startDate.HasValue && endDate.HasValue)
				transactions = transactions.Where(t => t.DateTime >= startDate && t.DateTime <= endDate).ToList();

			return transactions;
        }

        public async Task<Transaction> AddTransaction(string userId, Transaction transaction)
        {
            if (transaction.Category != null)
            {
                var category = await _context.GetCategorByIdWithAccessCheckAsync(userId, transaction.Category.Id);

                if (category == null)
                    throw new EntityNotFoundException();
            }

            if (transaction.Account == null)
                throw new System.ArgumentException();

			var account = await _context.GetAccountByIdWithAccessCheck(userId, transaction.AccountId);

			if (account == null)
				throw new EntityNotFoundException();

            var newTransaction = await _dal.Transactions.Add(transaction);

            return newTransaction;
        }

        public async Task<Transaction> PatchTransaction(string userId, Transaction transaction)
        {
            Category category = null;

            if (transaction.Category != null)
            {
                category = await _context.GetCategorByIdWithAccessCheckAsync(userId, transaction.Category.Id);

                if (category == null)
                    throw new EntityNotFoundException();
            }

            BankAccount account = null;

            if (transaction.Account != null)
            {
				account = await _context.GetAccountByIdWithAccessCheck(userId, transaction.AccountId);

				if (account == null)
					throw new EntityNotFoundException();
			}

            var newTransaction = await _dal.Transactions.Update(transaction);

            return newTransaction;
        }

        public async Task DeleteTransaction(string userId, int transactionId)
        {
            var user = await _dal.Users.GetById(userId);

			var transaction = await GetByIdWithChecks(userId, transactionId);

			await _dal.Transactions.Delete(transaction);
        }

		private async Task<Transaction> GetByIdWithChecks(string userId, int id)
		{
			var transaction = await _dal.Transactions.GetById(id);

			if (transaction == null)
				throw new EntityNotFoundException();

			await CheckAccessAndThrowException(userId, transaction);

			return transaction;
		}

		private async Task CheckAccessAndThrowException(string userId, Transaction transaction)
		{
			var user = await _context.Users.FindAsync(userId);

			await CheckAccessAndThrowException(user, transaction);
		}

		private async Task CheckAccessAndThrowException(ApplicationUser user, Transaction transaction)
		{
			var trustee = await _context.TrusteeAccounts
				.Where(t => t.TrusteeId == user.TrusteeId)
				.Join(_context.Transactions, t => t.AccountId, tx => tx.AccountId, (t, tx) => tx)
				.Where(a => a.Id == transaction.Id)
				.FirstOrDefaultAsync();

			if (trustee == null)
				throw new EntityAccessDeniedException();
		}
	}
}
