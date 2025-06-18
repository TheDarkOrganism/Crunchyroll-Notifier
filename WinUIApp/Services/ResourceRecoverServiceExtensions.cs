namespace WinUIApp.Services
{
	internal static class ResourceRecoverServiceExtensions
	{
		public static IHostBuilder AddResourceRecovery(this IHostBuilder hostBuilder, string file)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));

			return hostBuilder.ConfigureServices((context, services) => services.AddHostedService(_ => new ResourceRecoveryService(file, context.HostingEnvironment.ContentRootPath)));
		}
	}
}
