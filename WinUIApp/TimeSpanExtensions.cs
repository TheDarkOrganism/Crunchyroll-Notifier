namespace WinUIApp
{
	internal static class TimeSpanExtensions
	{
		public static TimeSpan Clamp(this TimeSpan value, TimeSpan min, TimeSpan max)
		{
			return value < min ? min : value > max ? max : value;
		}
	}
}
