using System.Linq;
using System.Text;

namespace Audiobooks.WebApi
{
	public static class Extensions
	{
		public static StringBuilder AppendLineWithTabs(this StringBuilder sb, string value, int tabsCount)
		{
			var tabs = string.Join(null, Enumerable.Repeat("\t", tabsCount));
			return sb.AppendLine($"{tabs}{value}");
		}

		public static bool IsNullOrWhiteSpace(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}
	}
}