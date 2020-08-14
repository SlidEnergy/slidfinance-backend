using System;
using System.Collections.Generic;
using System.Text;

namespace SlidFinance.App.Utils
{
	public class StringUtils
	{
		public static string ToBase64StringWithUrlAndFilenameSafe(byte[] inArray)
		{
			// https://tools.ietf.org/html/rfc3548#page-6
			string base64 = Convert.ToBase64String(inArray);
			return base64.Replace("+", "-").Replace("/", "_");
		}
	}
}
