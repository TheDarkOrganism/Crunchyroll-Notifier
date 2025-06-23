namespace WinUIApp.Controls
{
	public sealed partial class CustomTimePicker : UserControl
	{
		private readonly IEnumerable<int> _range24 = Enumerable.Range(0, 24);

		private readonly IEnumerable<int> _range60 = Enumerable.Range(0, 60);

		private int _hours;

		private int _minutes;

		private int _seconds;

		public static readonly DependencyProperty TimeProperty = DependencyProperty.Register(nameof(Time), typeof(TimeSpan), typeof(CustomTimePicker), new(default(TimeSpan), static (d, e) =>
		{
			if (d is CustomTimePicker control && e.NewValue is TimeSpan timeSpan)
			{
				control._hours = timeSpan.Hours;
				control._minutes = timeSpan.Minutes;
				control._seconds = timeSpan.Seconds;

				control.UpdateDisplay();
			}
		}));

		public TimeSpan Time
		{
			get => (TimeSpan)GetValue(TimeProperty);
			set => SetValue(TimeProperty, value);
		}

		private void UpdateDisplay()
		{
			TimeContent.Text = Time.ToString(@"hh\:mm\:ss");
		}

		public CustomTimePicker()
		{
			InitializeComponent();
			UpdateDisplay();
		}

		private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Time = new(_hours, _minutes, _seconds);

			TimeButton.Flyout.Hide();
		}
	}
}
