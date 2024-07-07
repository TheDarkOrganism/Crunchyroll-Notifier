using App.Attributes;

namespace App
{
	internal sealed class FeedConfig
	{
		#region Properties

		[JsonRequired]
		[TimeSpanValidation(1)]
		public TimeSpan Interval { get; init; } = TimeSpan.FromSeconds(10);

		[JsonRequired]
		[Range(1, 100)]
		public int MaxNotifications { get; init; } = 10;

		[JsonRequired]
		public bool ShowFirstRun { get; init; }

		[JsonRequired]
		public Visibility Visibility { get; init; }

		public IEnumerable<string> Dubs { get; init; } = [];

		public IEnumerable<string> Names { get; init; } = [];

		#endregion
	}
}
