using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;

namespace SlidFinance.App.Saltedge
{
	public class SaltedgeService : ISaltedgeService
	{
		private IApplicationDbContext _context;

		public SaltedgeService(IApplicationDbContext context)
		{
			_context = context;
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
	}
}
