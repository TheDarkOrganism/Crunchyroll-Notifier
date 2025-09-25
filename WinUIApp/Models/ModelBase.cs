namespace WinUIApp.Models
{
	internal abstract class ModelBase : IModelBase
	{
		[JsonIgnore]
		public bool Modified { get; private set; }

		[JsonIgnore]
		public bool ReloadConfiguration { get; private set; }

		protected void OnModified(bool reloadConfiguration = false)
		{
			Modified = true;
			ReloadConfiguration = reloadConfiguration;
		}

		public void MarkReloaded()
		{
			ReloadConfiguration = false;
		}

		public void MarkUnmodified()
		{
			Modified = false;
		}
	}
}
