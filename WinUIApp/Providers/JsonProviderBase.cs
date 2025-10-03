
namespace WinUIApp.Providers
{
	internal abstract class JsonProviderBase<T> : JsonConfigurationProvider
		where T : JsonProviderBase<T>
	{
		protected readonly ILogger<T> Logger;

		protected JsonProviderBase(JsonConfigurationSource jsonConfigurationSource, ILogger<T> logger) : base(jsonConfigurationSource)
		{
			ArgumentNullException.ThrowIfNull(logger, nameof(logger));

			Logger = logger;
		}

		public override void Load(Stream stream)
		{
			string? file = Source.Path;

			try
			{
				base.Load(stream);
			}
			catch (JsonException ex)
			{
				Logger.LogWarning(ex, "Failed to read JSON from {File}.", file);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Unable to read {File}.", file);
			}
		}
	}
}
