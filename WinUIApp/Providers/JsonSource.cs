namespace WinUIApp.Providers
{
	internal sealed class JsonSource : JsonConfigurationSource
	{
		private readonly ILogger<JsonProvider> _logger;

		public JsonSource(string file, bool optional, bool reloadOnChange)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));

			Path = file;
			Optional = optional;
			ReloadOnChange = reloadOnChange;

			_logger = LoggerProvider.GetLogger<JsonProvider>();
		}

		public override IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			EnsureDefaults(builder);

			return new JsonProvider(this, _logger);
		}
	}
}
