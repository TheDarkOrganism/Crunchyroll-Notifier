namespace WinUIApp.Options
{
	internal static class OptionsExtensions
	{
		public static IHostBuilder AddFileModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TOptions, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TIOptions>(this IHostBuilder hostBuilder, FileModel<TOptions, TIOptions> fileModel, Func<IServiceCollection, IConfigurationSection, IServiceCollection> configure)
			where TOptions : ModelBase, TIOptions
			where TIOptions : class, IModelBase
		
		{
			return hostBuilder.ConfigureServices((context, services) => configure(services, context.Configuration.GetSection(fileModel.ConfigurationSection)).AddSingleton<IOptions<TIOptions>>(static provider => provider.GetRequiredService<IOptions<TOptions>>()).AddSingleton<IFileModel<TIOptions>>(fileModel));
		}

		public static IHostBuilder ConfigureSavableJson<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TIOptions>(this IHostBuilder hostBuilder)
			where TIOptions : class, IModelBase
		{
			return hostBuilder.ConfigureServices(static services => services.AddSingleton<ISavableJsonOptions<TIOptions>, SavableJsonOptions<TIOptions>>());
		}
	}
}
