using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Audiobooks.ImportJob
{
	public class Genre
	{
		private string _name;

		private Genre()
		{
		}

		[JsonProperty(PropertyName = "id")]
		public int Id { get; set; }

		[JsonProperty(PropertyName = "name")]
		public string Name
		{
			get => _name;
			private set => _name = value.WhiteSpaceToNull();
		}

		public SqlDataRecord ToSqlDataRecord()
		{
			return new SqlDataRecord(new[]
			{
				new SqlMetaData("Id", SqlDbType.Int),
				new SqlMetaData("Name", SqlDbType.NVarChar, SqlMetaData.Max)
			})
			.SetValuesFluent(Id, Name);
		}
	}

	public class GenreEqualityComparer : IEqualityComparer<Genre>
	{
		public bool Equals(Genre x, Genre y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}

			return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
		}

		public int GetHashCode(Genre obj)
		{
			return obj.Name?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;
		}
	}
}