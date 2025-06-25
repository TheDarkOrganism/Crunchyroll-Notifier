using System.Text;

namespace WinUIApp.Attributes
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	internal sealed class TimeSpanRangeAttribute : ValidationAttribute
	{
		private readonly string _mininumMessage;

		private readonly string _maxinumMessage;

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

			_mininumMessage = Format(Mininum);
			_maxinumMessage = Format(Maxinum);
		}

		public TimeSpanRangeAttribute(int minDays, int maxDays, int minHours, int maxHours, int minMinutes, int maxMinutes) : this(minDays, maxDays, minHours, maxHours, minMinutes, maxMinutes, 0, 0) { }

		public TimeSpanRangeAttribute(int minDays, int maxDays, int minHours, int maxHours) : this(minDays, maxDays, minHours, maxHours, 0, 0) { }

		public TimeSpanRangeAttribute(int minDays, int maxDays) : this(minDays, maxDays, 0, 0) { }

		public override bool IsValid(object? value)
		{
			return (value is TimeSpan timeSpan || TimeSpan.TryParse(value?.ToString(), out timeSpan)) && timeSpan.Clamp(Mininum, Maxinum) == timeSpan;
		}

		private static string GetFormatEnd(int count)
		{
			return count == 1 ? string.Empty : "s";
		}

		private static string Format(TimeSpan timeSpan)
		{
			StringBuilder stringBuilder = new();

			const string format = "{0} {1}{2} ";

			int days = timeSpan.Days;

			if (days != 0)
			{
				stringBuilder = stringBuilder.AppendFormat(format, days, "day", GetFormatEnd(days));
			}

			int hours = timeSpan.Hours;

			if (hours != 0)
			{
				stringBuilder = stringBuilder.AppendFormat(format, hours, "hour", GetFormatEnd(hours));
			}

			int minutes = timeSpan.Minutes;

			if (minutes != 0)
			{
				stringBuilder = stringBuilder.AppendFormat(format, minutes, "minute", GetFormatEnd(minutes));
			}

			int seconds = timeSpan.Seconds;

			if (seconds != 0)
			{
				stringBuilder = stringBuilder.AppendFormat(format, seconds, "second", GetFormatEnd(seconds));
			}

			return stringBuilder.ToString().Trim();
		}

		public override string FormatErrorMessage(string name)
		{
			return string.Format("The field {0} must be between ({1}) and ({2}).", name, _mininumMessage, _maxinumMessage);
		}
	}
}