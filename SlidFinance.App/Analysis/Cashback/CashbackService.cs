using SlidFinance.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SlidFinance.App.Analysis
{
	public class CashbackService : ICashbackService
	{
		private IApplicationDbContext _context;

		public CashbackService(IApplicationDbContext context)
		{
			_context = context;
		}

		private int[] ConvertToMccCodes(string searchString)
		{
			var regex = new Regex(@"\b\d{4}\b", RegexOptions.IgnoreCase);
			var matches = regex.Matches(searchString);
			var codes = matches.Select(x => Convert.ToInt32(x.Value)).ToArray();

			return codes;
		}

		public async Task<List<WhichCardToPay>> WhichCardToPay(string userId, string searchString)
		{
			var codes = ConvertToMccCodes(searchString);

			// Все счета пользователя
			var accounts = await _context.GetAccountListWithAccessCheckAsync(userId);

			var categories = await GetCashbackCategoriesByMccCodesAsync(accounts, codes);

			// Получаем информацию о величине кэшбэка
			var categoriesWithCashback = AddCashbackPercentInfo(categories);

			// Нам интересен только максимальный кэшбэк
			var categoriesWithMaxCashback = FilterCategoriesWithMaxCashback(categoriesWithCashback);

			// Получаем дополнительную информацию
			var whichCardToPay = AddAdditionalInfo(categoriesWithMaxCashback, accounts);

			return whichCardToPay;
		}

		private async Task<List<CashbackCategoryByMccCode>> GetCashbackCategoriesByMccCodesAsync(List<BankAccount> accounts, int[] mccCodes)
		{
			// Все категории кэшбэков по тарифам
			var allUserCategories = await _context.CashbackCategories
				.Join(accounts, cat => cat.TariffId, a => a.SelectedTariffId, (cat, a) => cat)
				.ToListAsync();

			// Запрашиваем все категории где упоминаются наши MCC
			var allCategoriesByMcc = await _context.CashbackCategoryMcc
				.Where(m => mccCodes.Contains(m.MccCode))
				.Join(allUserCategories, m => m.CategoryId, c => c.Id, (m, c) => new CashbackCategoryByMccCode()
				{
					MccCode = m.MccCode.ToString(),
					Id = c.Id,
					Title = c.Title,
					TariffId = c.TariffId,
					Type = c.Type
				}).ToListAsync();

			var increasedCashbackCategories = allCategoriesByMcc.Where(x => x.Type == CashbackCategoryType.IncreasedCashback).ToList();
			var noCashbackCategories = allCategoriesByMcc.Where(x => x.Type == CashbackCategoryType.NoCashback).ToList();

			var baseCashbackCategories = (from code in mccCodes
										  from cat in allUserCategories
										  where cat.Type == CashbackCategoryType.BaseCashback
										  select new CashbackCategoryByMccCode()
										  {
											  MccCode = code.ToString(),
											  Id = cat.Id,
											  Title = cat.Title,
											  TariffId = cat.TariffId,
											  Type = cat.Type
										  }).ToList();

			// Ищем кэшбэк в категории базового кэшбэка за исключением категории без кэшбэка и в категориях повышенного кэшбэка
			var comparer = new CashbackCategoryByMccCodeEqualityComparer();
			var categories = increasedCashbackCategories.Union(baseCashbackCategories.Except(noCashbackCategories, comparer), comparer).ToList();

			// TODO: отфильтровать только действующие категории

			return categories;
		}

		private List<CashbackCategoryByMccCodeWithCashbackPercent> AddCashbackPercentInfo(List<CashbackCategoryByMccCode> categories)
		{
			return categories.GroupJoin(_context.Cashback, cat => cat.Id, cb => cb.CategoryId, (cat, cbs) => new CashbackCategoryByMccCodeWithCashbackPercent()
			{
				SearchPart = cat.MccCode,
				CategoryTitle = cat.Title,
				Percent = cbs.Max(x => x.Percent),
				TariffId = cat.TariffId,
			}).ToList();
		}

		private List<WhichCardToPay> AddAdditionalInfo(List<CashbackCategoryByMccCodeWithCashbackPercent> categoriesWithMaxCashback, List<BankAccount> accounts)
		{
			return (from cat in categoriesWithMaxCashback
					join tar in _context.Tariffs on cat.TariffId equals tar.Id
					join prod in _context.Products on tar.ProductId equals prod.Id
					join bank in _context.Banks on prod.BankId equals bank.Id
					join acc in accounts on cat.TariffId equals acc.SelectedTariffId
					select new WhichCardToPay
					{
						SearchPart = cat.SearchPart,
						CategoryTitle = cat.CategoryTitle,
						Percent = cat.Percent,
						BankTitle = bank.Title,
						AccountTitle = acc.Title
					}).ToList();
		}

		private List<T> FilterCategoriesWithMaxCashback<T>(List<T> categoriesWithCashback) where T : CashbackCategoryByMccCodeWithCashbackPercent
		{
			return (from cat in categoriesWithCashback
					group cat by new
					{
						SearchPart = cat.SearchPart,
					} into g
					select new
					{
						SearchPart = g.Key.SearchPart,
						Categories = g.Where(x => x.SearchPart == g.Key.SearchPart && x.Percent == g.Max(y => y.Percent)).FirstOrDefault()
					})
												 .Select(x => x.Categories)
												 .ToList();
		}
	}
}
