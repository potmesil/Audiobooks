using Microsoft.SqlServer.Server;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace Audiobooks.ImportJob
{
	public class Author
	{
		private string _firstName;
		private string _lastName;

		private Author()
		{
		}

		[JsonProperty(PropertyName = "id")]
		public int Id { get; private set; }

		[JsonProperty(PropertyName = "first_name")]
		public string FirstName
		{
			get => _firstName;
			private set => _firstName = value.WhiteSpaceToNull();
		}

		[JsonProperty(PropertyName = "last_name")]
		public string LastName
		{
			get => _lastName;
			private set => _lastName = value.WhiteSpaceToNull();
		}

		public SqlDataRecord ToSqlDataRecord()
		{
			return new SqlDataRecord(new[]
			{
				new SqlMetaData("Id", SqlDbType.Int),
				new SqlMetaData("FirstName", SqlDbType.NVarChar, SqlMetaData.Max),
				new SqlMetaData("LastName", SqlDbType.NVarChar, SqlMetaData.Max)
			})
			.SetValuesFluent(Id, FirstName, LastName);
		}
	}

	public class AuthorEqualityComparer : IEqualityComparer<Author>
	{
		public bool Equals(Author x, Author y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}

			return x.Id == y.Id;
		}

		public int GetHashCode(Author obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}