using Microsoft.EntityFrameworkCore;
using SlidFinance.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.App
{
    public class RulesService : IRulesService
	{
		private DataAccessLayer _dal;
		private IApplicationDbContext _context;

		public RulesService(DataAccessLayer dal, IApplicationDbContext context)
		{
            _dal = dal;
			_context = context;
		}

        public async Task<List<Rule>> GetListWithAccessCheckAsync(string userId)
        {
			var rules = await _context.GetRuleListWithAccessCheckAsync(userId);

            return rules.Distinct().ToList();
        }

        public async Task<Rule> AddRule(string userId, int? accountId, string bankCategory, int? categoryId, string description, int? mcc)
        {
            BankAccount account = null;

            if (accountId.HasValue)
            {
				account = await _context.GetAccountByIdWithAccessCheck(userId, accountId.Value);
			}

            Category category = null;

            if (categoryId.HasValue)
            {
                category = await _context.GetCategorByIdWithAccessCheckAsync(userId, categoryId.Value);

                if (category == null)
                    throw new EntityNotFoundException();
            }

            var rules = await _context.GetRuleListWithAccessCheckAsync(userId);

            var order = rules.Any() ? rules.Max(x => x.Order) + 1 : 0;

            var rule = await _dal.Rules.Add(new Rule(account, bankCategory, category, description, mcc, order));

            return rule;
        }

        public async Task<Rule> EditRule(string userId, int ruleId, int? accountId, string bankCategory, int? categoryId, string description, int? mcc)
        {
			var editRule = await _context.GetRuleByIdWithAccessCheckAsync(userId, ruleId);

			if (editRule == null)
				throw new EntityNotFoundException();

			BankAccount account = null;

            if (accountId.HasValue)
            {
				account = await _context.GetAccountByIdWithAccessCheck(userId, accountId.Value);

				if (account == null)
					throw new EntityNotFoundException();
			}

            Category category = null;

            if (categoryId.HasValue)
            {
                category = await _context.GetCategorByIdWithAccessCheckAsync(userId, categoryId.Value);

                if (category == null)
                    throw new EntityNotFoundException();
            }

            editRule.Account = account;
            editRule.Category = category;
            editRule.BankCategory = bankCategory;
            editRule.Description = description;
            editRule.Mcc = mcc;

            await _dal.Rules.Update(editRule);

            return editRule;
        }

        public async Task DeleteRule(string userId, int bankId)
        {
            var rule = await _context.GetRuleByIdWithAccessCheckAsync(userId, bankId);

			if (rule == null)
				throw new EntityNotFoundException();

            await _dal.Rules.Delete(rule);
        }

        public async Task<List<GeneratedRule>> GenerateRules(string userId)
        {
            var transactions = await _context.GetTransactionListWithAccessCheckAsync(userId);

            var generatedRules = transactions
                .Where(x => x.Category != null)
                .GroupBy(x => new { AccountId = x.Account.Id, x.BankCategory, x.Description, x.MccId })
                .Select(x => new GeneratedRule
                {
                    AccountId = x.Key.AccountId,
                    BankCategory = x.Key.BankCategory,
                    Description = x.Key.Description,
                    MccId = x.Key.MccId,
                    Categories = x.Where(s => s.Category != null).Select(s => s.Category.Id)
                        .GroupBy(s => s)
                        .Select(s => new CategoryDistribution { CategoryId = s.Key, Count = s.Count() }).ToArray(),
                    Count = x.Count()
                })
                .Where(x => x.Count >= 5).ToList();

            var existRules = await _context.GetRuleListWithAccessCheckAsync(userId);

            var existGeneratedRules = existRules
                .Select(x => new GeneratedRule
                {
                    AccountId = x.AccountId,
                    BankCategory = x.BankCategory,
                    Description = x.Description,
                    MccId = x.Mcc,
                }).ToList();

            generatedRules = generatedRules.Except(existGeneratedRules, new RuleComparer()).ToList();

            return generatedRules.ToList();
        }
	}
}
