namespace App
{
	internal sealed class FeedConfig
	{
		#region Properties

		public double Interval { get; init; } = 10;

		public int MaxNotifications { get; init; } = 10;

		public bool ShowFirstRun { get; init; }

		public Visibility Visibility { get; init; }

		public IEnumerable<string> Dubs { get; init; } = [];

		public IEnumerable<string> Names { get; init; } = [];

		#endregion

		#region Methods

		public bool ShouldSerializeDubs()
		{
			return Dubs.Any();
		}

		public bool ShouldSerializeNames()
		{
			return Names.Any();
		}

		#endregion
	}
}
