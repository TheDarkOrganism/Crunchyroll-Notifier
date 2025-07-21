namespace WinUIApp.Models
{
	internal sealed class LastUpdateModel : ModelBase, ILastUpdateModel
	{
		private DateTime _lastUpdate = DateTime.UtcNow;

		[JsonPropertyName("lastUpdate")]
		public DateTime LastUpdate
		{
			get => _lastUpdate;
			set
			{
				if (value != default && _lastUpdate != value)
				{
					_lastUpdate = value;

					OnModified(false);
				}
			}
		}
	}
}
