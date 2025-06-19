namespace WinUIApp.Models
{
	public sealed partial class ConfigModel : ValidationModelBase
	{
		private TimeSpan _interval = TimeSpan.FromSeconds(30);

		[JsonRequired]
		[JsonConverter(typeof(Converters.TimeSpanConverter))]
		[JsonPropertyName("interval")]
		[TimeSpanRange(0, 1)]
		public TimeSpan Interval
		{
			get => _interval;
			set
			{
				if (_interval != value)
				{
					_interval = value;

					OnPropertyChanged(value);
				}
			}
		}

		private int _maxNotifications = 30;

		[JsonRequired]
		[JsonPropertyName("maxNotifications")]
		[Range(1, 100)]
		public int MaxNotifications
		{
			get => _maxNotifications;
			set
			{
				if (_maxNotifications != value)
				{
					_maxNotifications = value;

					OnPropertyChanged(value);
				}
			}
		}

		private bool _showFirstRun = true;

		[JsonRequired]
		[JsonPropertyName("showFirstRun")]
		public bool ShowFirstRun
		{
			get => _showFirstRun;
			set
			{
				if (_showFirstRun != value)
				{
					_showFirstRun = value;

					OnPropertyChanged(value);
				}
			}
		}

		private VisibilityType _visibility;

		[JsonRequired]
		[JsonConverter(typeof(Converters.EnumConverter))]
		[JsonPropertyName("visibility")]
		[EnumDataType(typeof(VisibilityType))]
		public VisibilityType Visibility
		{
			get => _visibility;
			set
			{
				if (_visibility != value)
				{
					_visibility = value;

					OnPropertyChanged(value);
				}
			}
		}

		private FeedHostType _feedHostType;

		[JsonRequired]
		[JsonConverter(typeof(Converters.EnumConverter))]
		[JsonPropertyName("feedHost")]
		[EnumDataType(typeof(FeedHostType))]
		public FeedHostType FeedHost
		{
			get => _feedHostType;
			set
			{
				if (_feedHostType != value)
				{
					_feedHostType = value;

					OnPropertyChanged(value);
				}
			}
		}

		[JsonPropertyName("dubs")]
		public ObservableCollection<string> Dubs { get; } = [];

		[JsonPropertyName("names")]
		public ObservableCollection<string> Names { get; } = [];

		private static void RemoveEmptyValues(ObservableCollection<string> values)
		{
			foreach (string value in values)
			{
				if (string.IsNullOrWhiteSpace(value))
				{
					_ = values.Remove(value);
				}
			}
		}

		public ConfigModel()
		{
			ValidateProperty(Interval);
			ValidateProperty(MaxNotifications);
			ValidateProperty(Visibility);
			ValidateProperty(FeedHost);

			RemoveEmptyValues(Dubs);
			RemoveEmptyValues(Names);
		}
	}
}
