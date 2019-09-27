using System;
using System.Collections.Generic;
using System.Text;

namespace SlidFinance.App
{
	/// <summary>
	/// Режим доступа
	/// </summary>
	public enum AccessMode
	{
		/// <summary>
		/// Полный доступ к системе
		/// </summary>
		All = 0,

		/// <summary>
		/// Доступен только импорт транзакций
		/// </summary>
		Import = 1,
	}
}
