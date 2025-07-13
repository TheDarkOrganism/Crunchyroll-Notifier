namespace WinUIApp.Models
{
	public abstract class ModelBase
	{
		[JsonIgnore]
		internal bool Modified { get; private set; }

		[JsonIgnore]
		internal bool ReloadConfiguration { get; private set; }

		protected void OnModified(bool reloadConfiguration)
		{
			Modified = true;
			ReloadConfiguration = reloadConfiguration;
		}

		internal void MarkUnmodified()
		{
			Modified = false;
			ReloadConfiguration = false;
		}
	}
}
