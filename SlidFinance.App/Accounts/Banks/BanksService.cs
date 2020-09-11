using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class BanksService : IBanksService
	{
		private IApplicationDbContext _context;

		public BanksService(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<Bank>> GetLis()
		{
			var banks = await _context.Banks.ToListAsync();

			return banks.OrderBy(x => x.Title).ToList();
		}

		public async Task<Bank> AddBank(string title)
		{
			var bank = new Bank(title);
			_context.Banks.Add(bank);
			await _context.SaveChangesAsync();

			return bank;
		}

		public async Task<Bank> EditBank(int bankId, string title)
		{
			var editBank = await _context.Banks.FindAsync(bankId);

			editBank.Rename(title);

			_context.Banks.Update(editBank);
			await _context.SaveChangesAsync();

			return editBank;
		}

		public async Task DeleteBank(int bankId)
		{
			var bank = await _context.Banks.FindAsync(bankId);

			_context.Banks.Remove(bank);
			await _context.SaveChangesAsync();
		}
	}
}
