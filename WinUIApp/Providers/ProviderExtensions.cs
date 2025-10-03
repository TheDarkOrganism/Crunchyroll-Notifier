namespace WinUIApp.Providers
{
	internal static class ProviderExtensions
	{
		public static IConfigurationBuilder AddJsonFileWithValidation<TModel>(this IConfigurationBuilder configurationBuilder, string file, string section, bool optional = false, bool reloadOnChange = false)
			where TModel : notnull, ModelBase
		{
			return configurationBuilder.Add(new ValidationConfigurationSource<TModel>(file, section, optional, reloadOnChange));
		}

		public static IConfigurationBuilder AddJsonFileWithValidation<TModel, TIModel>(this IConfigurationBuilder configurationBuilder, FileModel<TModel, TIModel> fileModel, bool optional = false, bool reloadOnChange = false)
			where TModel : notnull, ModelBase, TIModel
			where TIModel : class, IModelBase
		{
			return AddJsonFileWithValidation<TModel>(configurationBuilder, fileModel.File, fileModel.ConfigurationSection, optional, reloadOnChange);
		}

		public static IConfigurationBuilder AddJson(this IConfigurationBuilder configurationBuilder, string file, bool optional = false, bool reloadOnChange = false)
		{
			return configurationBuilder.Add(new JsonSource(file, optional, reloadOnChange));
		}
	}
}
