using H.NotifyIcon;
using Microsoft.UI.Xaml.Input;
using Serilog;
using WinUIApp.Providers;
using WinUIApp.Services;
#if PACKAGED_APP
using Windows.ApplicationModel;
#endif

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIApp
{
	/// <summary>
	/// Provides application-specific behavior to supplement the default Application class.
	/// </summary>
	public sealed partial class App : Application
	{
		private readonly IHost _host;

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			const string lastUpdateFile = "LastUpdate.json";

			FileModel<ConfigModel, IConfigModel> configFileModel = new("appsettings.json", "Config");
			FileModel<LastUpdateModel, ILastUpdateModel> lastUpdateFileModel = new(lastUpdateFile);

			_host = Host.CreateDefaultBuilder()
				.ConfigureAppConfiguration((context, builder) => builder.AddJsonFileWithValidation(configFileModel, context).AddJsonFile(lastUpdateFile))
				.ConfigureServices(services => services.AddSingleton(AppNotificationManager.Default).AddSingleton<NotificationHelper>().AddSingleton<IFileModel<IConfigModel>>(configFileModel).AddSingleton<IFileModel<ILastUpdateModel>>(lastUpdateFileModel).AddHostedService<MainService>().AddHostedService<CrunchyrollService>().AddScoped<Settings>().AddScoped(static _ => new HttpClientHandler()).ConfigureHttpClientDefaults(static builder => builder.ConfigureHttpClient(static client => client.DefaultRequestHeaders.UserAgent.ParseAdd("chrome")).ConfigurePrimaryHttpMessageHandler(static provider => provider.GetRequiredService<HttpClientHandler>())))
				.ConfigureSavableJson(configFileModel)
				.ConfigureSavableJson(lastUpdateFileModel)
				.AddResourceRecovery<IConfigModel>()
				.AddResourceRecovery<ILastUpdateModel>()
				.UseSerilog((context, config) => config.MinimumLevel.Verbose()
						.Enrich.FromLogContext()
						.WriteTo.Debug()
						.WriteTo.Async(configure =>
						{
							ModelSerializerContext serializerContext = ModelSerializerContext.Default;

							IConfigurationSection section = context.Configuration.GetSection(configFileModel.ConfigurationSection);

							IConfigurationSection loggingSection = section.GetSection(serializerContext.GetJsonName(nameof(ConfigModel.UseLogging)));

							IConfigurationSection logLevelSection = section.GetSection(serializerContext.GetJsonName(nameof(ConfigModel.LogLevel)));

							_ = configure.Conditional(log => Enum.TryParse(logLevelSection.Value, out LogEventLevel logLevel) && log.Level >= logLevel && bool.TryParse(loggingSection.Value, out bool useLogging) && useLogging, configureSync => configureSync.File(Path.Combine(context.HostingEnvironment.ContentRootPath, "Logs", "Log-.txt"), shared: true, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7));
						})
				)
				.Build();

			InitializeComponent();
		}

		/// <summary>
		/// Invoked when the application is launched.
		/// </summary>
		/// <param name="args">Details about the launch request and process.</param>
		protected override async void OnLaunched(LaunchActivatedEventArgs args)
		{
			if (Resources.TryGetValue("Tray", out object trayValue) && trayValue is ResourceDictionary trayDictionary)
			{
				foreach ((object key, object value) in trayDictionary)
				{
					switch (value)
					{
						case XamlUICommand command:
							command.ExecuteRequested += key.ToString() switch
							{
								"Settings" => (_, _) => _host.Services.GetRequiredService<Settings>().Activate(),
								"Exit" => (_, _) => Exit(),
								_ => throw new NotImplementedException()
							};
							break;
						case TaskbarIcon trayIcon:
#if PACKAGED_APP
							trayIcon.ToolTipText = Package.Current.DisplayName;
#else
							trayIcon.ToolTipText = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>()?.Product ?? "App";
#endif

							trayIcon.ForceCreate();
							break;
						default:
							break;
					}
				}
			}
			else
			{
				throw new Exception($"Unable to find the tray {nameof(ResourceDictionary)} under the {nameof(Resources)}.");
			}

			AppDomain.CurrentDomain.ProcessExit += async (_, _) => await _host.StopAsync();

			await _host.RunAsync();
		}
	}
}
