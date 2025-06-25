namespace WinUIApp
{
	internal static class TimeSpanExtensions
	{
		public static TimeSpan Min(this TimeSpan timeSpan, TimeSpan other)
		{
			return timeSpan > other ? other : timeSpan;
		}

		public static TimeSpan Clamp(this TimeSpan value, TimeSpan min, TimeSpan max)
		{
			return value < min ? min : value > max ? max : value;
		}
	}
}
