using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SlidFinance.App.Utils
{
	public class EnumUtils
	{
		/// Получить указанный атрибут члена перечисления.
		/// </summary>
		/// <param name="enumObj">Значение перечисления.</param>
		/// <param name="attributeType">Тип атрибута.</param>
		/// <returns>Возвращает первый атрибут указанного типа или null, если атрибут не найден.</returns>
		public static object GetAttribute(Enum enumObj, Type attributeType)
		{
			if (enumObj == null)
				throw new ArgumentNullException(nameof(enumObj));

			if (!attributeType.IsSubclassOf(typeof(Attribute)))
				throw new ArgumentException("Тип должен быть атрибутом.", nameof(attributeType));

			// Получаем информацию о значении перечисления.
			FieldInfo fieldInfo = enumObj.GetType().GetField(enumObj.ToString());

			if (fieldInfo == null)
				return null;

			// Получаем список атрибутов (кроме унаследованных).
			object[] attribArray = fieldInfo.GetCustomAttributes(attributeType, false);

			// Если нашли хотя бы один атрибут, то возвращаем первый.
			return attribArray.FirstOrDefault();
		}

		/// <summary>
		/// Возвращает указанный атрибут члена перечисления.
		/// </summary>
		/// <typeparam name="TAttribute"></typeparam>
		/// <param name="enumObj"></param>
		/// <returns></returns>
		public static TAttribute GetAttribute<TAttribute>(Enum enumObj)
			=> (TAttribute)GetAttribute(enumObj, typeof(TAttribute));

		/// <summary>
		/// Получить описание члена перечисления.
		/// </summary>
		/// <param name="enumObj">Значение перечисления.</param>
		/// <returns>Возвращает строку с описанием, которая содержится в атрибуте <see cref="DescriptionAttribute"/>, или имя константы, если атрибут не найден.</returns>
		public static string GetDescription(Enum enumObj)
		{
			// Находим атрибут [Description], и возвращаем его значение.

			var attribute = (DescriptionAttribute)GetAttribute(enumObj, typeof(DescriptionAttribute));

			if (attribute != null)
				return attribute.Description;

			// Атрибут не найден - возвращаем текстовое представление значения перечисления.

			return enumObj.ToString();
		}

		/// <summary>
		/// Возвращает список
		/// </summary>
		/// <typeparam name="TEnum"></typeparam>
		/// <returns></returns>
		public static IEnumerable<TEnum> GetValues<TEnum>() => (TEnum[])Enum.GetValues(typeof(TEnum));
	}
}
