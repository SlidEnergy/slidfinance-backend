using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SlidFinance.WebApi
{
	/// <summary>
	/// Добавляет требования авторизации к операциям
	/// </summary>
	public class SecurityRequirementsOperationFilter : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			// Policy names map to scopes
			var requiredScopes = context.MethodInfo
				.GetCustomAttributes(true)
				.Union(context.MethodInfo.DeclaringType.GetCustomAttributes(true))
				.OfType<AuthorizeAttribute>()
				.Select(attr => attr.Policy)
				.Distinct();

			if (requiredScopes.Any())
			{
				operation.Responses.Add("401", new Response { Description = "Ошибка авторизации" });
				operation.Responses.Add("403", new Response { Description = "Доступ запрещен" });

				operation.Security = new List<IDictionary<string, IEnumerable<string>>>();
				operation.Security.Add(new Dictionary<string, IEnumerable<string>>
				{
					{ "Oauth2", requiredScopes.Where(x => x != null) },
					{ "Bearer", requiredScopes.Where(x => x != null) }
				});
			}
		}
	}
}
