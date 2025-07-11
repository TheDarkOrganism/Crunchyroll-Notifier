
namespace WinUIApp.Providers
{
	internal static class ProviderExtensions
	{
		public static IConfigurationBuilder AddJsonFileWithValidation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(this IConfigurationBuilder configurationBuilder, string file, IFileProvider fileProvider)
			where T : notnull
		{
			return configurationBuilder.Add(new ValidationConfigurationSource<T>(file, fileProvider));
		}
	}
}
