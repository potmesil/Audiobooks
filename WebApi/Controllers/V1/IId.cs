using System.Collections.Generic;

namespace Audiobooks.WebApi.Controllers.V1
{
	public interface IId
	{
		int Id { get; }
	}

	public class IdEqualityComparer<T> : IEqualityComparer<T> where T : class, IId
	{
		public bool Equals(T x, T y)
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

		public int GetHashCode(T obj)
		{
			return obj.Id.GetHashCode();
		}
	}
}