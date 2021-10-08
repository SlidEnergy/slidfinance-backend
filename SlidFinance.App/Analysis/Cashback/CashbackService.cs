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

		private int[] findMccCodesInString(string searchString)
		{
			var regex = new Regex(@"\b\d{4}\b", RegexOptions.IgnoreCase);
			var matches = regex.Matches(searchString);
			var codes = matches.Select(x => Convert.ToInt32(x.Value)).ToArray();

			return codes;
		}

		private string[] findSearchPartInStringWithoutMcc(string searchString, int[] mcc)
		{
			var parts = searchString.Split(" ");
			return parts.Except(mcc.Select(x => x.ToString())).ToArray();
		}

		public async Task<List<WhichCardToPay>> WhichCardToPayAsync(string userId, string searchString)
		{
			var codes = findMccCodesInString(searchString);

			var parts = findSearchPartInStringWithoutMcc(searchString, codes);

			// Все счета пользователя
			var accounts = await _context.GetBankAccountListWithAccessCheckAsync(userId);

			// Все категории кэшбэков по тарифам
			var allUserCategories = await _context.GetCashbackCategoriesWithAccessCheckAsync(userId);

			var categoriesByMcc = await GetCashbackCategoriesByMccAsync(allUserCategories, codes);

			var categoriesByTitle = GetCashbackCategoriesByTitleAsync(allUserCategories, parts);

			//var categoriesByMerchant = await GetCashbackCategoriesByMerchantAsync(accounts, codes);

			var categories = categoriesByMcc.Union(categoriesByTitle).ToList();

			// Получаем информацию о величине кэшбэка
			var categoriesWithCashback = AddCashbackPercentInfo(categories);

			// Нам интересен только максимальный кэшбэк
			var categoriesWithMaxCashback = FilterCategoriesWithMaxCashback(categoriesWithCashback);

			// Получаем дополнительную информацию
			var whichCardToPay = AddAdditionalInfo(categoriesWithMaxCashback, accounts);

			return whichCardToPay;
		}

		private async Task<List<CashbackCategoryBySearchPart>> GetCashbackCategoriesByMccAsync(List<CashbackCategory> allUserCategories, int[] mccCodes)
		{
			var allCategoriesMcc = await _context.CashbackCategoryMcc.Where(x => allUserCategories.Select(c => c.Id).Contains(x.CategoryId)).ToListAsync();

			// Запрашиваем все категории где упоминаются наши MCC
			var allCategoriesByMcc = allCategoriesMcc
				.Where(m => mccCodes.Contains(m.MccCode))
				.Join(allUserCategories, m => m.CategoryId, c => c.Id, (m, c) => new CashbackCategoryBySearchPart()
				{
					SearchPart = m.MccCode.ToString(),
					Id = c.Id,
					Title = c.Title,
					TariffId = c.TariffId,
					Type = c.Type
				}).ToList();

			var increasedCashbackCategories = allCategoriesByMcc.Where(x => x.Type == CashbackCategoryType.IncreasedCashback).ToList();
			var noCashbackCategories = allCategoriesByMcc.Where(x => x.Type == CashbackCategoryType.NoCashback).ToList();

			var baseCashbackCategories = (from code in mccCodes
										  from cat in allUserCategories
										  where cat.Type == CashbackCategoryType.BaseCashback
										  select new CashbackCategoryBySearchPart()
										  {
											  SearchPart = code.ToString(),
											  Id = cat.Id,
											  Title = cat.Title,
											  TariffId = cat.TariffId,
											  Type = cat.Type
										  }).ToList();

			// Ищем кэшбэк в категории базового кэшбэка за исключением категории без кэшбэка и в категориях повышенного кэшбэка
			var comparer = new CashbackCategoryBySearchPartEqualityComparer();
			var categories = increasedCashbackCategories.Union(baseCashbackCategories.Except(noCashbackCategories, comparer), comparer).ToList();

			// TODO: отфильтровать только действующие категории

			return categories;
		}

		private List<CashbackCategoryBySearchPart> GetCashbackCategoriesByTitleAsync(List<CashbackCategory> allUserCategories, string[] parts)
		{
			// Запрашиваем все категории где есть совпадения в названии
			var increasedCashbackCategories = new List<CashbackCategoryBySearchPart>();

			foreach (var part in parts) {
				foreach (var category in allUserCategories)
				{
					if (category.Title.Contains(part, StringComparison.CurrentCultureIgnoreCase))
						increasedCashbackCategories.Add(new CashbackCategoryBySearchPart()
						{
							SearchPart = part,
							Id = category.Id,
							Title = category.Title,
							TariffId = category.TariffId,
							Type = category.Type
						});
				}
			}

			var baseCashbackCategories = (from part in parts
										  from cat in allUserCategories
										  where cat.Type == CashbackCategoryType.BaseCashback
										  select new CashbackCategoryBySearchPart()
										  {
											  SearchPart = part,
											  Id = cat.Id,
											  Title = cat.Title,
											  TariffId = cat.TariffId,
											  Type = cat.Type
										  }).ToList();

			// Ищем кэшбэк в категории базового кэшбэка за исключением категории без кэшбэка и в категориях повышенного кэшбэка
			var comparer = new CashbackCategoryBySearchPartEqualityComparer();
			var categories = increasedCashbackCategories.Union(baseCashbackCategories, comparer).ToList();

			// TODO: отфильтровать только действующие категории

			return categories;
		}

		private List<CashbackCategoryBySearchPartWithCashbackPercent> AddCashbackPercentInfo(List<CashbackCategoryBySearchPart> categories)
		{
			return categories.GroupJoin(_context.Cashback, cat => cat.Id, cb => cb.CategoryId, (cat, cbs) => new CashbackCategoryBySearchPartWithCashbackPercent()
			{
				SearchPart = cat.SearchPart,
				CategoryTitle = cat.Title,
				Percent = cbs.Max(x => x.Percent),
				TariffId = cat.TariffId,
			}).ToList();
		}

		private List<WhichCardToPay> AddAdditionalInfo(List<CashbackCategoryBySearchPartWithCashbackPercent> categoriesWithMaxCashback, List<BankAccount> accounts)
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

		private List<T> FilterCategoriesWithMaxCashback<T>(List<T> categoriesWithCashback) where T : CashbackCategoryBySearchPartWithCashbackPercent
		{
			return (from cat in categoriesWithCashback
					group cat by new
					{
						SearchPart = cat.SearchPart,
					} into g
					select new
					{
						SearchPart = g.Key.SearchPart,
						Categories = g.Where(x => x.SearchPart == g.Key.SearchPart && x.Percent == g.Max(y => y.Percent)).ToList()
					})
					.SelectMany(x => x.Categories)
					.ToList();
		}
	}
}
