
namespace WinUIApp.Providers
{
	internal static class ProviderExtensions
	{
		public static IConfigurationBuilder AddJsonFileWithValidation<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] TIModel>(this IConfigurationBuilder configurationBuilder, IFileModel<TIModel> fileModel, HostBuilderContext context)
			where TIModel : class, IModelBase
		{
			return configurationBuilder.Add(new ValidationConfigurationSource<TIModel>(fileModel, context));
		}
	}
}
