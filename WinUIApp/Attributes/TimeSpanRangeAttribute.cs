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

		public TimeSpanRangeAttribute(int minDays = 0, int maxDays = int.MaxValue, int minHours = 0, int maxHours = 24, int minMinutes = 0, int maxMinutes = 60, int minSeconds = 0, int maxSeconds = 60, bool dayLock = false)
		{
			ValidateParams(minDays, maxDays, int.MaxValue);
			ValidateParams(minHours, maxHours, 24);
			ValidateParams(minMinutes, maxMinutes, 60);
			ValidateParams(minSeconds, maxSeconds, 60);

			Mininum = new(minDays, minHours, minMinutes, minSeconds);
			Maxinum = new(maxDays, maxHours, maxMinutes, maxSeconds);

			if (dayLock)
			{
				Maxinum = Maxinum.Min(TimeSpan.FromDays(maxDays));
			}

			_mininumMessage = Format(Mininum);
			_maxinumMessage = Format(Maxinum);
		}

		private static void ValidateParam(int value, int limit, string paramName)
		{
			ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limit, nameof(limit));

			ArgumentOutOfRangeException.ThrowIfGreaterThan(value, limit, paramName);
			ArgumentOutOfRangeException.ThrowIfLessThan(value, -limit, paramName);
		}

		private static void ValidateParams(int min, int max, int limit, [CallerArgumentExpression(nameof(min))] string? minParamName = null, [CallerArgumentExpression(nameof(max))] string? maxParamName = null)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(minParamName, nameof(minParamName));
			ArgumentException.ThrowIfNullOrWhiteSpace(maxParamName, nameof(maxParamName));

			ArgumentOutOfRangeException.ThrowIfGreaterThan(min, max, minParamName);
			ValidateParam(min, limit, minParamName);
			ArgumentOutOfRangeException.ThrowIfLessThan(max, min, maxParamName);
			ValidateParam(max, limit, maxParamName);
		}

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

			return stringBuilder.ToString().TrimEnd();
		}

		public override string FormatErrorMessage(string name)
		{
			return string.Format("The field {0} must be between ({1}) and ({2}).", name, _mininumMessage, _maxinumMessage);
		}
	}
}