namespace WinUIApp.Models
{
	internal sealed class LastUpdateModel : ModelBase
	{
		private DateTime _lastUpdate = DateTime.Now;

		[JsonPropertyName("lastUpdate")]
		public DateTime LastUpdate
		{
			get => _lastUpdate;
			set
			{
				if (value != default && _lastUpdate != value)
				{
					_lastUpdate = value;

					OnModified();
				}
			}
		}
	}
}
