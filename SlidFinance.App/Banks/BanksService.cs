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

		public async Task<List<Bank>> GetLis()
        {
			var banks = await _context.Banks.ToListAsync();

			return banks.OrderBy(x => x.Title).ToList();
        }

        public async Task<Bank> AddBank(string title)
        {
            var bank = await _dal.Banks.Add(new Bank(title));

            return bank;
        }

        public async Task<Bank> EditBank(int bankId, string title)
        {
            var editBank = await _dal.Banks.GetById(bankId);

            editBank.Rename(title);

            await _dal.Banks.Update(editBank);

            return editBank;
        }

        public async Task DeleteBank(int bankId)
        {
            var bank = await _dal.Banks.GetById(bankId);

            await _dal.Banks.Delete(bank);
        }
    }
}
