using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace SlidFinance.WebApi
{
	/// <summary>
	/// Фильтр схемы для перечисления. 
	/// Описывает перечисления как отдельная модель, что позволяет избегать дублирования. 
	/// Используется ссылка на модель (#/definitions/enumType")
	/// По умолчанию перечисления описываются в том месте где они используются.
	/// </summary>
	public class EnumAsModelSchemaFilter : ISchemaFilter
	{
		/// <summary>
		/// Применяет фильтр для переданного типа.
		/// </summary>
		public void Apply(OpenApiSchema model, SchemaFilterContext context)
		{
			if (model.Properties== null)
				return;

			var enumProperties = model.Properties.Where(p => p.Value.Enum != null)
				.Union(model.Properties.Where(p => p.Value.Items?.Enum != null)).ToList();
			var enums = context.ApiModel.Type.GetMembers(BindingFlags.Public | BindingFlags.Instance)
				.Where(p => p.MemberType == MemberTypes.Field || p.MemberType == MemberTypes.Property)
				.Select(m => {
					Type propOrFieldType = m.MemberType == MemberTypes.Property ? ((PropertyInfo)m).PropertyType : ((FieldInfo)m).FieldType;

					return Nullable.GetUnderlyingType(propOrFieldType) ?? propOrFieldType.GetElementType() ??
									propOrFieldType.GetGenericArguments().FirstOrDefault() ?? propOrFieldType;
				})
				.Where(p => p.IsEnum)
				.Distinct()
				.ToList();

			foreach (var enumProperty in enumProperties)
			{
				var enumPropertyValue = enumProperty.Value.Enum != null ? enumProperty.Value : enumProperty.Value.Items;

				var enumValues = enumPropertyValue.Enum.Select(e => $"{e}").ToList();
				var enumType = enums.SingleOrDefault(p =>
				{
					var enumNames = Enum.GetNames(p);
					if (enumNames.Except(enumValues, StringComparer.InvariantCultureIgnoreCase).Any())
						return false;
					if (enumValues.Except(enumNames, StringComparer.InvariantCultureIgnoreCase).Any())
						return false;
					return true;
				});

				if (enumType == null)
					throw new Exception($"Property {enumProperty} not found in {context.ApiModel.Type.Name} Type.");

				if (context.SchemaRegistry.Definitions.ContainsKey(enumType.Name) == false)
					context.SchemaRegistry.Definitions.Add(enumType.Name, enumPropertyValue);

				var schema = new OpenApiSchema
				{
					Reference = new OpenApiReference { }
					Ref = $"#/definitions/{enumType.Name}"
				};
				if (enumProperty.Value.Enum != null)
				{
					model.Properties[enumProperty.Key] = schema;
				}
				else if (enumProperty.Value.Items?.Enum != null)
				{
					enumProperty.Value.Items = schema;
				}
			}
		}
	}
}