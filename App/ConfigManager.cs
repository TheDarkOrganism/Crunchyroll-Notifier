using App.Converters;
using System.Text.Json.Serialization.Metadata;

namespace App
{
	internal static class ConfigManager
	{
		#region Fields

		#region Private

		private const string _feedConfig = "FeedConfig.json";

		private static readonly Lazy<FeedConfig> _fallback = new(() => new());

		private static readonly JsonSerializerOptions _serializerOptions = new()
		{
			Converters =
			{
				new EnumConverter(),
				new TimeSpanConverter()
			},
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			PropertyNameCaseInsensitive = true,
			PropertyNamingPolicy = new PascalCaseNamingPolicy(),
			TypeInfoResolver = new DefaultJsonTypeInfoResolver()
			{
				Modifiers =
				{
					typeInfo =>
					{
						foreach (JsonPropertyInfo jsonPropertyInfo in typeInfo.Properties)
						{
							Action<object, object?>? setProperty = jsonPropertyInfo.Set;

							if (setProperty is not null)
							{
								IEnumerable<ValidationAttribute>? attributes = jsonPropertyInfo.AttributeProvider?.GetCustomAttributes(typeof(ValidationAttribute), true).OfType<ValidationAttribute>();

								if (attributes is not null)
								{
									jsonPropertyInfo.Set = (obj, value) =>
									{
										foreach(ValidationAttribute attribute in attributes)
										{
											if (!attribute.IsValid(value))
											{
												throw new InvalidDataException(attribute.FormatErrorMessage(jsonPropertyInfo.Name));
											}
										}

										setProperty(obj, value);
									};
								}
							}
						}
					}
				}
			},
			WriteIndented = true
		};

		#endregion

		#region Public

		public static event Action<FeedConfig>? OnFeedConfigChanged;

		#endregion

		#endregion


		#region Methods

		#region Private

		private static async ValueTask<bool> HandleSaveAsync(FeedConfig config, string filename, CancellationToken token)
		{
			try
			{
				await File.WriteAllTextAsync(filename, JsonSerializer.Serialize(config, _serializerOptions), token);

				return true;
			}
			catch
			{
				return false;
			}
		}

		private static void OpenFile(string filename)
		{
			_ = Process.Start(new ProcessStartInfo(filename)
			{
				UseShellExecute = true
			});
		}

		#endregion

		#region Public

		public static async ValueTask<(LoadStatus, FeedConfig)> LoadAsync(CancellationToken token)
		{
			if (!File.Exists(_feedConfig))
			{
				return (LoadStatus.NotFound, _fallback.Value);
			}

			try
			{
				FeedConfig? feedConfig = JsonSerializer.Deserialize<FeedConfig>(await File.ReadAllTextAsync(_feedConfig, token), _serializerOptions);

				if (JsonSerializer.Deserialize<FeedConfig>(await File.ReadAllTextAsync("FeedConfig.json", token), _serializerOptions) is FeedConfig config)
				{
					return (LoadStatus.Loaded, config);
				}
			}
			catch { }

			return (LoadStatus.Invalid, _fallback.Value);
		}

		public static async ValueTask<bool> SaveAsync(FeedConfig config, CancellationToken token)
		{
			if (await HandleSaveAsync(config, _feedConfig, token))
			{
				OnFeedConfigChanged?.Invoke(config);

				return true;
			}
			else
			{
				return false;
			}
		}

		public static async ValueTask OpenConfigAsync(CancellationToken token)
		{
			if (File.Exists(_feedConfig) || await SaveAsync(_fallback.Value, token))
			{
				OpenFile(_feedConfig);
			}
		}

		public static async ValueTask OpenDefaultConfigAsync(CancellationToken token)
		{
			const string example = "FeedConfigDefault.json";

			if (File.Exists(example) || await HandleSaveAsync(_fallback.Value, example, token))
			{
				try
				{
					File.SetAttributes(example, FileAttributes.ReadOnly);

					OpenFile(example);
				}
				catch { }
			}
		}

		#endregion

		#endregion
	}
}
