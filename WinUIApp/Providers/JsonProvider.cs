namespace WinUIApp.Providers
{
	internal sealed partial class JsonProvider(JsonConfigurationSource jsonConfigurationSource, ILogger<JsonProvider> logger) : JsonProviderBase<JsonProvider>(jsonConfigurationSource, logger)
	{
		public override void Load(Stream stream)
		{
			if (stream.Length > 2)
			{
				base.Load(stream);
			}
		}
	}
}
