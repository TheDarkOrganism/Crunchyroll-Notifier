namespace WinUIApp.Models
{
	public interface IConfigModel : IValidationModelBase
	{
		TimeSpan Interval { get; set; }

		int MaxNotifications { get; set; }

		bool ShowFirstRun { get; set; }

		VisibilityType Visibility { get; set; }

		FeedHostType FeedHost { get; set; }

		bool UseLogging { get; set; }

		ObservableCollection<string> Dubs { get; }

		ObservableCollection<string> Names { get; }
	}
}
