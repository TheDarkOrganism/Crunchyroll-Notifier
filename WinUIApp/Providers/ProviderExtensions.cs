
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

		public static IConfigurationBuilder AddJson(this IConfigurationBuilder configurationBuilder, string file, bool optional = false, bool reloadOnChange = false)
		{
			return configurationBuilder.Add(new JsonSource(file, optional, reloadOnChange));
		}
	}
}
