
namespace WinUIApp.Providers
{
	internal static class ProviderExtensions
	{
		public static IConfigurationBuilder AddJsonFileWithValidation<TModel, TIModel>(this IConfigurationBuilder configurationBuilder, FileModel<TModel, TIModel> fileModel, HostBuilderContext context)
			where TModel : ModelBase, TIModel
			where TIModel : class, IModelBase
		{
			return configurationBuilder.Add(new ValidationConfigurationSource<TModel, TIModel>(fileModel, context));
		}
	}
}
