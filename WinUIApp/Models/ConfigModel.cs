using System.Collections.Specialized;

namespace WinUIApp.Models
{
	internal sealed partial class ConfigModel : ValidationModelBase<ConfigModel>, IConfigModel
	{
		private TimeSpan _interval = TimeSpan.FromSeconds(30);

		private const int _minSeconds = 10;

		[JsonRequired]
		[TimeSpanRange(maxDays: 1, minSeconds: _minSeconds, dayLock: true)]
		public TimeSpan Interval
		{
			get => ContainsErrors() ? TimeSpan.FromSeconds(_minSeconds) : _interval;
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

		private bool _useLogging;

		public bool UseLogging
		{
			get => _useLogging;
			set
			{
				if (_useLogging != value)
				{
					_useLogging = value;

					OnPropertyChanged(value, true);
				}
			}
		}

		private LogEventLevel _logLevel = LogEventLevel.Information;

		[JsonRequired]
		[EnumDataType(typeof(LogEventLevel))]
		[AllowedValues(LogEventLevel.Information, LogEventLevel.Warning, LogEventLevel.Error, LogEventLevel.Fatal)]
		public LogEventLevel LogLevel
		{
			get => _logLevel;
			set
			{
				if (_logLevel != value)
				{
					_logLevel = value;

					OnPropertyChanged(value, true);
				}
			}
		}

		[Language]
		[NotEmpty]
		public ObservableCollection<string> Dubs { get; } = [];

		[NotEmpty]
		public ObservableCollection<string> Names { get; } = [];

		private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e, string propertyName)
		{
			if (sender is ObservableCollection<string> values && e.NewItems is IList newItems)
			{
				foreach (string value in newItems.OfType<string>())
				{
					if (!IsValid(value, propertyName))
					{
						_ = values.Remove(value);
					}
				}
			}
		}

		public ConfigModel()
		{
			ValidateProperty(Interval);
			ValidateProperty(MaxNotifications);
			ValidateProperty(Visibility);
			ValidateProperty(FeedHost);
			ValidateProperty(LogLevel);

			Dubs.CollectionChanged += (sender, e) => OnCollectionChanged(sender, e, nameof(Dubs));
			Names.CollectionChanged += (sender, e) => OnCollectionChanged(sender, e, nameof(Names));
		}
	}
}
