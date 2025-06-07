using H.NotifyIcon;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

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
			_host = Host.CreateDefaultBuilder()
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
								"Settings" => (_, _) => { },
								"Exit" => (_, _) => Exit(),
								_ => throw new NotImplementedException()
							};
							break;
						case TaskbarIcon trayIcon:
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
