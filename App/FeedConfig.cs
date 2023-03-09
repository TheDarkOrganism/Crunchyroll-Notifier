namespace App
{
	internal sealed class FeedConfig
	{
		#region Properties

		public double Interval { get; init; }

		public int MaxNotifications { get; init; }

		public bool ShowFirstRun { get; init; }

		public Visibility Visibility { get; init; }

		public IEnumerable<string> Dubs { get; init; }

		public IEnumerable<string> Names { get; init; }

		#endregion

		public FeedConfig()
		{
			Interval = 10;
			MaxNotifications = 10;
			Dubs = Enumerable.Empty<string>();
			Names = Enumerable.Empty<string>();
		}

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
