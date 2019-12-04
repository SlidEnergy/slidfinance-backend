using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class BanksService : IBanksService
	{
        private DataAccessLayer _dal;
		private IApplicationDbContext _context;

		public BanksService(DataAccessLayer dal, IApplicationDbContext context)
        {
            _dal = dal;
			_context = context;
		}

		public async Task<List<Bank>> GetListWithAccessCheckAsync(string userId)
        {
			var user = await _context.Users.FindAsync(userId);

			var banks = await _context.TrusteeAccounts.Where(x => x.TrusteeId == user.TrusteeId)
				.Join(_context.Accounts, t => t.AccountId, a => a.Id, (t, a) => a)
				.Join(_context.Banks, a => a.BankId, b => b.Id, (a, b) => b)
				.ToListAsync();

			return banks.Distinct().OrderBy(x => x.Title).ToList();
        }

        public async Task<Bank> AddBank(string userId, string title)
        {
            var user = await _dal.Users.GetById(userId);

            var bank = await _dal.Banks.Add(new Bank(title, user));

            return bank;
        }

        public async Task<Bank> EditBank(string userId, int bankId, string title)
        {
            var editBank = await _dal.Banks.GetById(bankId);

            if (!editBank.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

            editBank.Rename(title);

            await _dal.Banks.Update(editBank);

            return editBank;
        }

        public async Task DeleteBank(string userId, int bankId)
        {
            var user = await _dal.Users.GetById(userId);

            var bank = await _dal.Banks.GetById(bankId);

            bank.IsBelongsTo(userId);

            await _dal.Banks.Delete(bank);
        }
    }
}
