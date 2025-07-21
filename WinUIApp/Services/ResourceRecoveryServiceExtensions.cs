namespace WinUIApp.Services
{
	internal static class ResourceRecoveryServiceExtensions
	{
		public static IHostBuilder AddResourceRecovery<TIModel>(this IHostBuilder hostBuilder)
			where TIModel : class, IModelBase
		{
			return hostBuilder.ConfigureServices(static services => services.AddHostedService<ResourceRecoveryService<TIModel>>());
		}
	}
}
