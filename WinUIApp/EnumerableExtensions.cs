namespace WinUIApp
{
	internal static class EnumerableExtensions
	{
		public static IEnumerable<T> DynamicSkip<T>(this IEnumerable<T> values, int count)
		{
			return count == 0 ? values : count > 0 ? values.Skip(count) : values.SkipLast(Math.Abs(count));
		}
	}
}
