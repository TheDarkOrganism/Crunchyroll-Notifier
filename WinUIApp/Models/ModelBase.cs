namespace WinUIApp.Models
{
	public abstract class ModelBase : IModelBase
	{
		[JsonIgnore]
		public bool Modified { get; private set; }

		[JsonIgnore]
		public bool ReloadConfiguration { get; private set; }

		protected void OnModified(bool reloadConfiguration)
		{
			Modified = true;
			ReloadConfiguration = reloadConfiguration;
		}

		public void MarkUnmodified()
		{
			Modified = false;
			ReloadConfiguration = false;
		}
	}
}
