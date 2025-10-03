namespace WinUIApp.Models
{
	internal interface IModelBase
	{
		bool Modified { get; }

		void MarkUnmodified();
	}
}
