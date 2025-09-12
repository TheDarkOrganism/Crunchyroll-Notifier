namespace WinUIApp.Models
{
	internal interface IModelBase
	{
		bool Modified { get; }

		bool ReloadConfiguration { get; }

		void MarkReloaded();

		void MarkUnmodified();
	}
}
