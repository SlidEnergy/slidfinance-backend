using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace SlidFinance.WebApi
{
	/// <summary>
	/// Добавляет требования авторизации к операциям
	/// </summary>
	public class SecurityRequirementsOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
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
				operation.Responses.Add("401", new OpenApiResponse { Description = "Ошибка авторизации" });
				operation.Responses.Add("403", new OpenApiResponse { Description = "Доступ запрещен" });

				var oAuthScheme = new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Oauth2" }
				};

				var bearerScheme = new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
				};

				operation.Security = new List<OpenApiSecurityRequirement> {

					new OpenApiSecurityRequirement
					{
						[oAuthScheme] = requiredScopes.Where(x => x != null).ToList()
					},
					new OpenApiSecurityRequirement
					{
						[bearerScheme] = requiredScopes.Where(x => x != null).ToList()
					}
				};

			}
		}
	}
}
