using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SaltEdgeNetCore.Client;
using SaltEdgeNetCore.Models.Connections;
using SlidFinance.Domain;

namespace SlidFinance.App.Saltedge
{
	public class SaltedgeService : ISaltedgeService
	{
		private IApplicationDbContext _context;
		private readonly ISaltEdgeClientV5 _saltedge;

		public SaltedgeService(IApplicationDbContext context, ISaltEdgeClientV5 saltedge)
		{
			_context = context;
			_saltedge = saltedge;
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

		public async Task<IEnumerable<SeConnection>> GetConnections(string userId)
		{
			var saltedgeAccount = await _context.GetSaltedgeAccountByIdWithAccessCheck(userId);

			var response = _saltedge.ConnectionsList(saltedgeAccount.CustomerId);

			return response.Data;
		}
	}
}
