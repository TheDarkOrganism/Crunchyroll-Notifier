using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace Console
{
	internal sealed class Program
	{
		private const string _feedConfigPath = "FeedConfig.json";

		public static async void Run(CancellationToken token)
		{
			static void Notification(Action<ToastContentBuilder> action)
			{
				ToastContentBuilder builder = new();

				action(builder);

				builder.Show();
			}

			FeedConfig? config = default;

			try
			{
				if (File.Exists(_feedConfigPath))
				{
					JToken json = JToken.Parse(await File.ReadAllTextAsync(_feedConfigPath, token));

					if (json.IsValid(JSchema.Parse(await File.ReadAllTextAsync("FeedConfigSchema.json", token))))
					{
						config = json.ToObject<FeedConfig>();
					}
				}
			}
			catch
			{
			}
			finally
			{
				if (config is null)
				{
					config = new();

					if (!File.Exists(_feedConfigPath))
					{
						await File.WriteAllTextAsync(_feedConfigPath, JsonConvert.SerializeObject(config), token);
					}

					const string example = "FeedConfigDefault.json";

					if (!File.Exists(example))
					{
						await File.WriteAllTextAsync(example, JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented, new StringEnumConverter()), token);
						File.SetAttributes(example, FileAttributes.ReadOnly);
					}

					Notification(builder => builder
					.AddText("Config Not Valid")
					.AddText("Using Default Config")
					.AddButton(new ToastButton()
					.SetContent("View Defualt Config")
					.AddArgument("json", example)));
				}
			}

			const string lastFile = "Last";

			DateTime last = default;

			if (File.Exists(lastFile))
			{
				if (DateTime.TryParse(await File.ReadAllTextAsync(lastFile, token), out DateTime result))
				{
					last = result;
				}
				else
				{
					last = DateTime.Now;
				}
			}
			else
			{
				last = DateTime.Now;

				if (config.ShowFirstRun)
				{
					Notification(builder => builder
						.AddText("App First Run")
						.AddButton(new ToastButton()
						.SetContent("Open Config")
						.AddArgument("json", "config"))
					);
				}

				await File.WriteAllTextAsync(lastFile, last.ToString(), token);
			}

			PeriodicTimer timer = new(TimeSpan.FromSeconds(config.Interval));

			static bool CheckValue(string? value, IEnumerable<string> values)
			{
				return !values.Any() || values.Any(v => v.Equals(value, StringComparison.OrdinalIgnoreCase));
			}

			try
			{
				do
				{
					XPathDocument document = new("http://feeds.feedburner.com/crunchyroll/rss/anime");

					XPathNavigator navigator = document.CreateNavigator();

					XmlNamespaceManager manager = new(navigator.NameTable);

					manager.AddNamespace("media", "http://search.yahoo.com/mrss/");
					manager.AddNamespace("crunchyroll", "http://www.crunchyroll.com/rss");

					DateTime copy = last;

					foreach (XPathNavigator nav in navigator.Select($"//item[position() <= {config.MaxNotifications}]").OfType<XPathNavigator>().Reverse())
					{
						if (DateTime.TryParse(nav.SelectSingleNode($".//crunchyroll:{config.Visibility.ToString()?.ToLower()}PubDate", manager)?.Value, out DateTime result) && result >= copy)
						{
							string? dub = nav.SelectSingleNode(".//title", manager)?.Value is string title ? Regex.Match(title, @"\(([A-Za-z\-]+) Dub\)").Groups.Values.ElementAtOrDefault(1)?.Value : default;

							if (CheckValue(dub, config.Dubs) && CheckValue(nav.SelectSingleNode(".//crunchyroll:seriesTitle", manager)?.Value, config.Names) && nav.SelectSingleNode(".//link")?.Value is string url)
							{
								Notification(builder => builder
								.AddText($"{nav.SelectSingleNode(".//crunchyroll:seriesTitle", manager)?.Value}{(dub is null ? string.Empty : $" ({dub})")}")
								.AddText($"Episode {nav.SelectSingleNode(".//crunchyroll:episodeNumber", manager)?.ValueAsInt}")
								.AddButton("Open", ToastActivationType.Protocol, url));
							}

							last = result;
						}
					}

					if (copy != last)
					{
						await File.WriteAllTextAsync(lastFile, last.ToString(), token);
					}
				} while (await timer.WaitForNextTickAsync(token));
			}
			catch { }

			Environment.Exit(0);

			//Environment.ExitCode = Convert.ToInt32(HostFactory.Run(hostConfig =>
			//{
			//	_ = hostConfig.Service<Service>(service =>
			//	{
			//		service.ConstructUsing(_ => new(config));
			//		service.WhenStarted(s => s.Start());
			//		service.WhenStopped(s => s.Stop());
			//	});

			//	_ = hostConfig.RunAsLocalSystem();

			//	_ = hostConfig.StartAutomatically();

			//	hostConfig.SetServiceName("CrunchyrollNotifierService");
			//	hostConfig.SetDisplayName("Crunchyroll Notifier Service");
			//	hostConfig.SetDescription("A service that alerts the user about new episodes on crunchyroll.");
			//}));
		}

		[STAThread]
		private static void Main()
		{
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length >= 2)
			{
				Environment.Exit(0);
			}

			static void OpenFile(string path)
			{
				_ = Process.Start(new ProcessStartInfo(path)
				{
					UseShellExecute = true
				});
			}

			static void OpenConfig()
			{
				OpenFile(_feedConfigPath);
			}

			ToastNotificationManagerCompat.OnActivated += toastArgs =>
			{
				ToastArguments args = ToastArguments.Parse(toastArgs.Argument);

				if (args.TryGetValue("json", out string? value))
				{
					switch (value)
					{
						case "config":
							OpenConfig();
							break;
						default:
							OpenFile(value);
							break;
					}
				}
			};

			CancellationTokenSource source = new();

			Run(source.Token);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			ContextMenuStrip strip = new();

			ToolStripMenuItem config = new("Config")
			{
				Name = "Config"
			};

			config.Click += (_, _) => OpenConfig();

			strip.Items.Add(config);

			ToolStripMenuItem exit = new("Exit")
			{
				Name = "Exit"
			};

			exit.Click += (_, _) => source.Cancel();

			strip.Items.Add(exit);

			using NotifyIcon icon = new()
			{
				ContextMenuStrip = strip,
				Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
				Text = "Crunchyroll Notifier",
				Visible = true
			};

			Application.Run();

			icon.Visible = false;
		}
	}
}