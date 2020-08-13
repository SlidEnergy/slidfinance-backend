namespace SlidFinance.WebApi
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

		/// <summary>
		/// Доступен только экспорт
		/// </summary>
		Export = 2,
	}
}
