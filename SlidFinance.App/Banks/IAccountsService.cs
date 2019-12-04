using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IAccountsService
	{
		Task<BankAccount> AddAccount(string userId, int bankId, string title, string code, float balance, float creditLimit);
		Task DeleteAccount(string userId, int accountId);
		Task<BankAccount> EditAccount(string userId, int accountId, string title, string code, float balance, float creditLimit);
		Task<BankAccount> GetById(string userId, int id);
		Task<List<BankAccount>> GetListWithAccessCheck(string userId, int? bankId);
		Task<BankAccount> PatchAccount(string userId, BankAccount account);
	}
}