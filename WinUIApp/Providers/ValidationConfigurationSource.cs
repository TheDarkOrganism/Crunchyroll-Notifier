namespace WinUIApp.Providers
{
	internal sealed class ValidationConfigurationSource<T>(string file, IFileProvider fileProvider) : IConfigurationSource
		where T : notnull
	{
		public IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			return new ValidationConfigurationProvider<T>(file, fileProvider);
		}
	}
}
