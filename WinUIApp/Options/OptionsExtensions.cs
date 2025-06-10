using Microsoft.Extensions.DependencyInjection;

namespace WinUIApp.Options
{
	internal static class OptionsExtensions
	{
		public static IHostBuilder ConfigureSavableJson<TOptions>(this IHostBuilder hostBuilder, string file, string? section = null)
			where TOptions : class
		{
			return hostBuilder.ConfigureServices((context, services) =>
			{
				section ??= Path.GetFileNameWithoutExtension(file);

				_ = services.Configure<TOptions>(context.Configuration.GetRequiredSection(section), static options => options.ErrorOnUnknownConfiguration = false);

				_ = services.AddSingleton<ISavableJsonOptions<TOptions>>(provider => new SavableJsonOptions<TOptions>(file, section, context.HostingEnvironment.ContentRootFileProvider, provider.GetRequiredService<IOptions<TOptions>>()));
			});
		}
	}
}
