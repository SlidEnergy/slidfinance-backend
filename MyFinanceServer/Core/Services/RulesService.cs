using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyFinanceServer.Core
{
    public class RulesService
    {
        private DataAccessLayer _dal;

        public RulesService(DataAccessLayer dal)
        {
            _dal = dal;
        }

        public async Task<List<Rule>> GetList(string userId)
        {
            var rules = await _dal.Rules.GetListWithAccessCheck(userId);

            return rules.ToList();
        }

        public async Task<Rule> AddRule(string userId, int? accountId, string bankCategory, int? categoryId, string description, int? mcc)
        {
            var user = await _dal.Users.GetById(userId);

            BankAccount account = null;

            if (accountId.HasValue)
            {
                account = await _dal.Accounts.GetById(accountId.Value);

                if (account == null)
                    throw new EntityNotFoundException();

                if (!account.IsBelongsTo(userId))
                    throw new EntityAccessDeniedException();
            }

            Category category = null;

            if (categoryId.HasValue)
            {
                category = await _dal.Categories.GetById(categoryId.Value);

                if (category == null)
                    throw new EntityNotFoundException();

                if (!category.IsBelongsTo(userId))
                    throw new EntityAccessDeniedException();
            }

            var rules = await _dal.Rules.GetListWithAccessCheck(userId);

            var order = rules.Any() ? rules.Max(x => x.Order) + 1 : 0;

            var rule = await _dal.Rules.Add(new Rule(account, bankCategory, category, description, mcc, order));

            return rule;
        }

        public async Task<Rule> EditRule(string userId, int ruleId, int? accountId, string bankCategory, int? categoryId, string description, int? mcc)
        {
            var editRule = await _dal.Rules.GetById(ruleId);

            if (!editRule.IsBelongsTo(userId))
                throw new EntityAccessDeniedException();

            BankAccount account = null;

            if (accountId.HasValue)
            {
                account = await _dal.Accounts.GetById(accountId.Value);

                if (account == null)
                    throw new EntityNotFoundException();

                if (!account.IsBelongsTo(userId))
                    throw new EntityAccessDeniedException();
            }

            Category category = null;

            if (categoryId.HasValue)
            {
                category = await _dal.Categories.GetById(categoryId.Value);

                if (category == null)
                    throw new EntityNotFoundException();

                if (!category.IsBelongsTo(userId))
                    throw new EntityAccessDeniedException();
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
            var rule = await _dal.Rules.GetById(bankId);

            rule.IsBelongsTo(userId);

            await _dal.Rules.Delete(rule);
        }

        public async Task<List<Api.Dto.GeneratedRule>> GenerateRules(string userId)
        {
            var transactions = await _dal.Transactions.GetListWithAccessCheck(userId);

            var generatedRules = transactions
                .Where(x => x.Category != null)
                .GroupBy(x => new { AccountId = x.Account.Id, x.BankCategory, x.Description, x.Mcc })
                .Select(x => new Api.Dto.GeneratedRule
                {
                    AccountId = x.Key.AccountId,
                    BankCategory = x.Key.BankCategory,
                    Description = x.Key.Description,
                    Mcc = x.Key.Mcc,
                    Categories = x.Where(s => s.Category != null).Select(s => s.Category.Id)
                        .GroupBy(s => s)
                        .Select(s => new Api.Dto.CategoryDistribution { CategoryId = s.Key, Count = s.Count() }).ToArray(),
                    Count = x.Count()
                })
                .Where(x => x.Count >= 5);

            var existRules = await _dal.Rules.GetListWithAccessCheck(userId);

            var existGeneratedRules = existRules
                .Select(x => new Api.Dto.GeneratedRule
                {
                    AccountId = x.Account.Id,
                    BankCategory = x.BankCategory,
                    Description = x.Description,
                    Mcc = x.Mcc,
                });

            generatedRules = generatedRules.Except(existGeneratedRules, new RuleComparer()).ToList();

            return generatedRules.ToList();
        }
    }

    public class RuleComparer : IEqualityComparer<Api.Dto.GeneratedRule>
    {
        bool IEqualityComparer<Api.Dto.GeneratedRule>.Equals(Api.Dto.GeneratedRule x, Api.Dto.GeneratedRule y)
        {
            return ((x.AccountId == null || x.AccountId.Equals(y.AccountId)) &&
                    (string.IsNullOrEmpty(x.BankCategory) || x.BankCategory.Equals(y.BankCategory)) &&
                    (string.IsNullOrEmpty(x.Description) || x.Description.Equals(y.Description)) &&
                    (x.Mcc == null || x.Mcc.Equals(y.Mcc)));
        }

        int IEqualityComparer<Api.Dto.GeneratedRule>.GetHashCode(Api.Dto.GeneratedRule obj)
        {
            if (obj == null)
                return 0;

            return (obj.BankCategory ?? "").GetHashCode() * 1000 +
                   (obj.Description ?? "").GetHashCode() * 100 +
                   ((obj.AccountId ?? 0) + 1) * 10 +
                   obj.Mcc ?? 0;
        }
    }
}
