using System.Collections.Generic;
using System.Threading.Tasks;
using SlidFinance.Domain;

namespace SlidFinance.App
{
	public interface IRulesService
	{
		Task<Rule> AddRule(string userId, int? accountId, string bankCategory, int? categoryId, string description, int? mcc);
		Task DeleteRule(string userId, int bankId);
		Task<Rule> EditRule(string userId, int ruleId, int? accountId, string bankCategory, int? categoryId, string description, int? mcc);
		Task<List<GeneratedRule>> GenerateRules(string userId);
		Task<List<Rule>> GetListWithAccessCheckAsync(string userId);
	}
}