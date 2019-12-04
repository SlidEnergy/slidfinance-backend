using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface ITransactionsService
	{
		Task<Transaction> AddTransaction(string userId, Transaction transaction);
		Task DeleteTransaction(string userId, int transactionId);
		Task<Transaction> GetById(string userId, int id);
		Task<List<Transaction>> GetListWithAccessCheckAsync(string userId, int? categoryId = null, DateTime? startDate = null, DateTime? endDate = null);
		Task<Transaction> PatchTransaction(string userId, Transaction transaction);
	}
}