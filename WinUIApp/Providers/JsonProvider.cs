namespace WinUIApp.Providers
{
	internal sealed partial class JsonProvider : JsonConfigurationProvider
	{
		private readonly ILogger<JsonProvider> _logger;

		public JsonProvider(JsonConfigurationSource jsonConfigurationSource, ILogger<JsonProvider> logger) : base(jsonConfigurationSource)
		{
			ArgumentNullException.ThrowIfNull(logger, nameof(logger));

			_logger = logger;
		}

		public override void Load(Stream stream)
		{
			if (stream.Length <= 2)
			{
				return;
			}

			string? file = Source.Path;

			try
			{
				base.Load(stream);
			}
			catch (JsonException ex)
			{
				_logger.LogWarning(ex, "Failed to read JSON from {File}.", file);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unable to read {File}.", file);
			}
		}
	}
}
