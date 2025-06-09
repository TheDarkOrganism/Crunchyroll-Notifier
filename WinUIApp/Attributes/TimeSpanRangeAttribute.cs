namespace WinUIApp.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	internal sealed class TimeSpanRangeAttribute : ValidationAttribute
	{
		public TimeSpan Mininum { get; }

		public TimeSpan Maxinum { get; }

		public TimeSpanRangeAttribute(int minDays, int maxDays, int minHours, int maxHours, int minMinutes, int maxMinutes, int minSeconds, int maxSeconds)
		{
			ArgumentOutOfRangeException.ThrowIfGreaterThan(minDays, maxDays, nameof(minDays));
			ArgumentOutOfRangeException.ThrowIfLessThan(maxDays, minDays, nameof(maxDays));
			ArgumentOutOfRangeException.ThrowIfGreaterThan(minHours, maxHours, nameof(minHours));
			ArgumentOutOfRangeException.ThrowIfLessThan(maxHours, minHours, nameof(maxHours));
			ArgumentOutOfRangeException.ThrowIfGreaterThan(minMinutes, maxMinutes, nameof(minMinutes));
			ArgumentOutOfRangeException.ThrowIfLessThan(maxMinutes, minMinutes, nameof(maxMinutes));
			ArgumentOutOfRangeException.ThrowIfGreaterThan(minSeconds, maxSeconds, nameof(minSeconds));
			ArgumentOutOfRangeException.ThrowIfLessThan(maxSeconds, minSeconds, nameof(maxSeconds));

			Mininum = new(minDays, minHours, minMinutes, minSeconds);
			Maxinum = new(maxDays, maxHours, maxMinutes, maxSeconds);
		}

		public TimeSpanRangeAttribute(int minDays, int maxDays, int minHours, int maxHours, int minMinutes, int maxMinutes) : this(minDays, maxDays, minHours, maxHours, minMinutes, maxMinutes, 0, 0) { }

		public TimeSpanRangeAttribute(int minDays, int maxDays, int minHours, int maxHours) : this(minDays, maxDays, minHours, maxHours, 0, 0) { }

		public TimeSpanRangeAttribute(int minDays, int maxDays) : this(minDays, maxDays, 0, 0) { }

		public override bool IsValid(object? value)
		{
			return (value is TimeSpan timeSpan || TimeSpan.TryParse(value?.ToString(), out timeSpan)) && timeSpan.Clamp(Mininum, Maxinum) == timeSpan;
		}
	}
}