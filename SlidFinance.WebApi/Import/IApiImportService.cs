using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	public interface IApiImportService
	{
		Task<int> Import(string userId, PatchAccountDataBindingModel data);
	}
}