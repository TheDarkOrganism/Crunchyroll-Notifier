namespace WinUIApp.Providers
{
	internal static class ProviderExtensions
	{
		public static IConfigurationBuilder AddJsonFileWithValidation<T>(this IConfigurationBuilder configurationBuilder, string file, IFileProvider fileProvider)
			where T : notnull
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));
			ArgumentNullException.ThrowIfNull(fileProvider, nameof(fileProvider));

			return configurationBuilder.Add(new ValidationConfigurationSource<T>(file, fileProvider));
		}
	}
}
