using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace App
{
	internal static class ConfigManager
	{
		#region Fields

		#region Private

		private const string _feedConfig = "FeedConfig.json";

		private static readonly Lazy<FeedConfig> _fallback = new(() => new());

		private static readonly JsonSerializerSettings _settings = new()
		{
			NullValueHandling = NullValueHandling.Ignore,
			Formatting = Formatting.Indented,
			Converters =
			[
				new StringEnumConverter()
			]
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
				await File.WriteAllTextAsync(filename, JsonConvert.SerializeObject(config, _settings), token);

				return true;
			}
			catch
			{
				return default;
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
				JToken json = JToken.Parse(await File.ReadAllTextAsync(_feedConfig, token));

				if (json.IsValid(JSchema.Parse(await File.ReadAllTextAsync("FeedConfigSchema.json", token))) && json.ToObject<FeedConfig>() is FeedConfig config)
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
				return default;
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
