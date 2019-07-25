using Npgsql;
using System;

namespace MyFinanceServer
{
	internal class ConnectionStringFactory
	{
		public static string Get()
		{
			var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

			var databaseUri = new Uri(databaseUrl);
			var userInfo = databaseUri.UserInfo.Split(':');

			var builder = new NpgsqlConnectionStringBuilder
			{
				Host = databaseUri.Host,
				Port = databaseUri.Port,
				Username = userInfo[0],
				Password = userInfo[1],
				Database = databaseUri.LocalPath.TrimStart('/'),
				SslMode = databaseUri.Host == "localhost" ? SslMode.Disable : SslMode.Require,
				TrustServerCertificate = true
			};

			return builder.ToString();
		}
	}
}
