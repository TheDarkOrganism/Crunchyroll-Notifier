using App.Attributes;

namespace App
{
	internal sealed class FeedConfig
	{
		#region Fields

		public const double IntervalMin = 10;

		public const int MaxNotificationsMax = 50;

		#endregion

		#region Properties

		[JsonRequired]
		[TimeSpanValidation(IntervalMin)]
		public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(10);

		[JsonRequired]
		[Range(1, MaxNotificationsMax)]
		public int MaxNotifications { get; init; } = 10;

		[JsonRequired]
		public bool ShowFirstRun { get; init; }

		[JsonRequired]
		public Visibility Visibility { get; init; }

		[JsonRequired]
		public FeedHostType FeedHost { get; init; }

		public IEnumerable<string> Dubs { get; init; } = [];

		public IEnumerable<string> Names { get; init; } = [];

		#endregion
	}
}
