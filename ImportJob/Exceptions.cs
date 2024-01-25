using System;
using System.Runtime.Serialization;

namespace Audiobooks.ImportJob
{
	[Serializable]
	public class InvalidRequiredFieldsException : Exception
	{
		public InvalidRequiredFieldsException(params string[] invalidRequiredFields) : base($"Invalid or missing required {(invalidRequiredFields.Length > 1 ? "fields" : "field")} {string.Join(", ", invalidRequiredFields)}")
		{
		}

		protected InvalidRequiredFieldsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override string ToString()
		{
			return $"InvalidRequiredFieldsException: {Message}";
		}
	}

	[Serializable]
	public class InvalidOptionalFieldsException : Exception
	{
		public InvalidOptionalFieldsException(params string[] invalidOptionalFields) : base($"Invalid or missing optional {(invalidOptionalFields.Length > 1 ? "fields" : "field")} {string.Join(", ", invalidOptionalFields)}")
		{
		}

		protected InvalidOptionalFieldsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override string ToString()
		{
			return $"InvalidOptionalFieldsException: {Message}";
		}
	}
}