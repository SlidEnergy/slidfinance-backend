using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class MerchantService : IMerchantService
	{
		private readonly IApplicationDbContext _context;

		public MerchantService(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<Merchant> GetByIdWithAccessCheckAsync(string userId, int id)
		{
			return await _context.GetMerchantByIdWithAccessCheckAsync(userId, id);
		}

		public async Task<List<Merchant>> GetListAsync()
		{
			return await _context.Merchants.ToListAsync();
		}

		public async Task<List<Merchant>> GetListWithAccessCheckAsync(string userId)
		{
			return await _context.GetMerchantListWithAccessCheckAsync(userId);
		}

		public async Task<Merchant> AddAsync(Merchant merchant)
		{
			var existMerchant = await _context.Merchants.FirstOrDefaultAsync(x => x.MccId == merchant.MccId && x.Name == merchant.Name);
			if (existMerchant == null)
			{
				_context.Merchants.Add(merchant);
				await _context.SaveChangesAsync();
			}

			return merchant;
		}

		public async Task<Merchant> EditMerchant(string userId, Merchant merchant)
		{
			var editMerchant = await _context.GetMerchantByIdWithAccessCheckAsync(userId, merchant.Id);

			if (editMerchant == null)
				throw new EntityNotFoundException();

			var isAdmin = await _context.IsAdmin(userId);

			if (isAdmin)
				editMerchant.IsPublic = merchant.IsPublic;

			editMerchant.Name = merchant.Name;
			editMerchant.DisplayName = merchant.DisplayName;
			editMerchant.Address = merchant.Address;
			editMerchant.Updated = DateTime.Now;

			_context.Merchants.Update(editMerchant);
			await _context.SaveChangesAsync();

			return editMerchant;
		}

	}
}
