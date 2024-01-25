using Dapper;
using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Audiobooks.ImportJob
{
	public static class Extensions
	{
		public static string GetJsonPropertyName<T>(this T instance, string propertyName) where T : class
		{
			var jsonPropertyName = typeof(T).GetProperty(propertyName)?.GetCustomAttribute<JsonPropertyAttribute>(false)?.PropertyName;
			return jsonPropertyName.IsNullOrWhiteSpace() ? propertyName : jsonPropertyName;
		}

		public static bool IsNullOrWhiteSpace(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		public static string WhiteSpaceToNull(this string value)
		{
			return value.IsNullOrWhiteSpace() ? null : value.Trim();
		}

		public static bool IsWellFormedUriString(this string value, UriKind uriKind = UriKind.Absolute)
		{
			return Uri.IsWellFormedUriString(value, uriKind);
		}

		public static Task<int> UpsertImportJobFailureAsync(this IDbConnection dbConnection, int audiobookId, Exception exception, IDbTransaction transaction = null)
		{
			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append("MERGE [ImportJobFailure] AS [Target]");
			sqlBuilder.Append(" USING (VALUES (@AudiobookId, @Exception, @Severity)) AS [Source] ([AudiobookId], [Exception], [Severity])");
			sqlBuilder.Append(" ON ([Source].[AudiobookId] = [Target].[AudiobookId])");
			sqlBuilder.Append(" WHEN MATCHED THEN UPDATE SET [Exception] = [Source].[Exception], [Severity] = [Source].[Severity]");
			sqlBuilder.Append(" WHEN NOT MATCHED THEN INSERT ([AudiobookId], [Exception], [Severity]) VALUES ([Source].[AudiobookId], [Source].[Exception], [Source].[Severity]);");

			return dbConnection.ExecuteAsync(
				sql: sqlBuilder.ToString(),
				param: new
				{
					AudiobookId = audiobookId,
					Exception = exception.ToString().WhiteSpaceToNull(),
					Severity = exception is InvalidOptionalFieldsException ? 1 : exception is InvalidRequiredFieldsException ? 2 : 3
				},
				transaction: transaction);
		}

		public static SqlDataRecord SetValuesFluent(this SqlDataRecord sqlDataRecord, params object[] values)
		{
			sqlDataRecord.SetValues(values);
			return sqlDataRecord;
		}
	}
}