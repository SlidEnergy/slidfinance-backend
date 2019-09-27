using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace SlidFinance.WebApi
{
    /// <summary>
    /// Фильтр операции для ответов. Добавляет описание к ответам полученных от наших контроллеров.
    /// </summary>
    public class ResponseWithDescriptionOperationFilter : IOperationFilter
    {
        private Dictionary<int, string> _codes = new Dictionary<int, string>()
        {
            { 400, "Неверный запрос." },
            { 500, "Необработанная ошибка сервера." },
        };

        /// <summary>
        /// Применяет фильтр для переданного ApiDescription.
        /// </summary>
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (operation.Responses == null)
                operation.Responses = new Dictionary<string, Response>();

            foreach (int code in _codes.Keys)
            {
                if (!operation.Responses.ContainsKey(code.ToString()))
                    operation.Responses.Add(code.ToString(), new Response { Description = this._codes[code] });
            }
        }
    }
}