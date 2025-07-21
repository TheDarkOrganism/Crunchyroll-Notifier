namespace WinUIApp.Models
{
	public interface IModelBase
	{
		bool Modified { get; }

		bool ReloadConfiguration { get; }

		void MarkUnmodified();
	}
}
