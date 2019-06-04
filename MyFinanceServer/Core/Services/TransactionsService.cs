using MyFinanceServer.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public class TransactionsService
    {
        private DataAccessLayer _dal;

        public TransactionsService(DataAccessLayer dal)
        {
            _dal = dal;
        }

        public async Task<Transaction> GetById(string userId, int id)
        {
            var transactions = await _dal.Transactions.GetById(id);

            if (transactions == null)
                throw new EntityNotFoundException();

            if (!transactions.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

            return transactions;
        }

        public async Task<List<Transaction>> GetList(string userId, int? categoryId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
			if(categoryId.HasValue)
				ArgumentValidator.ValidateId(categoryId.Value);

			if(startDate.HasValue && endDate.HasValue)
				ArgumentValidator.ValidatePeriodAllowEqual(startDate.Value, endDate.Value);

            var transactions = await _dal.Transactions.GetListWithAccessCheck(userId);

			if (categoryId != null) 
				transactions = transactions.Where(t => t.Category != null && t.Category.Id == categoryId).ToList();

			if(startDate.HasValue && endDate.HasValue)
				transactions = transactions.Where(t => t.DateTime >= startDate && t.DateTime <= endDate).ToList();

			return transactions;
        }

        public async Task<Transaction> AddTransaction(string userId, Transaction transaction)
        {
            var user = await _dal.Users.GetById(userId);

            if (transaction.Category != null)
            {
                var category = await _dal.Categories.GetById(transaction.Category.Id);

                if (category == null)
                    throw new EntityNotFoundException();

                if (!category.IsBelongsTo(userId))
                    throw new EntityAccessDeniedException();
            }

            if (transaction.Account == null)
                throw new System.ArgumentException();

            var account = await _dal.Accounts.GetById(transaction.Account.Id);

            if (account == null)
                throw new EntityNotFoundException();

            if (!account.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

            var newTransaction = await _dal.Transactions.Add(transaction);

            return newTransaction;
        }

        public async Task<Transaction> PatchTransaction(string userId, Transaction transaction)
        {
            var user = await _dal.Users.GetById(userId);

            Category category = null;

            if (transaction.Category != null)
            {
                category = await _dal.Categories.GetById(transaction.Category.Id);

                if (category == null)
                    throw new EntityNotFoundException();

                if (!category.IsBelongsTo(userId))
                    throw new EntityAccessDeniedException();
            }

            BankAccount account = null;

            if (transaction.Account != null)
            {
                account = await _dal.Accounts.GetById(transaction.Account.Id);

                if (account == null)
                    throw new EntityNotFoundException();

                if (!account.IsBelongsTo(userId))
                    throw new EntityAccessDeniedException();
            }

            var newTransaction = await _dal.Transactions.Update(transaction);

            return newTransaction;
        }

        public async Task DeleteTransaction(string userId, int transactionId)
        {
            var user = await _dal.Users.GetById(userId);

            var transaction = await _dal.Transactions.GetById(transactionId);

            transaction.IsBelongsTo(userId);

            await _dal.Transactions.Delete(transaction);
        }
    }
}
