﻿using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
	public class MccService : IMccService
	{
		private IApplicationDbContext _context;

		public MccService(IApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<List<Mcc>> GetListAsync()
		{
			var mcc = await _context.Mcc.ToListAsync();

			return mcc.ToList();
		}

		public async Task<Mcc> GetByIdAsync(int id)
		{
			return await _context.Mcc.FindAsync(id);
		}

		public async Task<Mcc> AddAsync(Mcc mcc)
		{
			_context.Mcc.Add(mcc);
			await _context.SaveChangesAsync();

			return mcc;
		}

		public async Task AddMccIfNotExistAsync(ICollection<Mcc> mccList)
		{
			if (mccList == null || mccList.Count == 0)
				return;

			var existingMccList = await GetListAsync();

			foreach (var mcc in mccList)
			{
				var existingMcc = existingMccList.FirstOrDefault(x => x.Code == mcc.Code);
				if (existingMcc == null)
					await AddAsync(mcc);
			}
		}
	}
}
