using System.Collections.Specialized;

namespace WinUIApp.Models
{
	public sealed partial class ConfigModel : ValidationModelBase<ConfigModel>
	{
		private TimeSpan _interval = TimeSpan.FromSeconds(30);

		private const int _minSeconds = 10;

		[JsonRequired]
		[JsonConverter(typeof(Converters.TimeSpanConverter))]
		[JsonPropertyName("interval")]
		[TimeSpanRange(0, 1, 0, 0, 0, 0, _minSeconds, 60, dayLock: true)]
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
		[JsonConverter(typeof(Converters.EnumConverter<VisibilityType>))]
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
		[JsonConverter(typeof(Converters.EnumConverter<FeedHostType>))]
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

		private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
		{
			if (sender is ObservableCollection<string> values && e.NewItems is IList newItems)
			{
				foreach (string value in newItems.OfType<string>())
				{
					if (string.IsNullOrWhiteSpace(value))
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

			Dubs.CollectionChanged += OnCollectionChanged;
			Names.CollectionChanged += OnCollectionChanged;
		}
	}
}
