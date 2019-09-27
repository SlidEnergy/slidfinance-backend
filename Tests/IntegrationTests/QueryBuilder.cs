using System;
using System.Text;
using System.Web;

namespace SlidFinance.WebApi.IntegrationTests
{
	public static class QueryBuilder
	{
		private const string DATE_TIME_FORMAT = "yyyy-MM-ddThh:mm:ss";

		private static string ConvertParamToString(object param)
		{
			if (param.GetType() == typeof(DateTime))
				return ((DateTime)param).ToString(DATE_TIME_FORMAT);
			else
				return param.ToString();
		}

		public static string FromObject(object queryParams)
		{
			var query = HttpUtility.ParseQueryString(string.Empty);

			foreach (var field in queryParams.GetType().GetProperties())
			{
				query[field.Name] = ConvertParamToString(field.GetValue(queryParams));
			}

			return query.ToString();
		}
	}
}
