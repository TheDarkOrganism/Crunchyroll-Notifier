using System.Xml;
using System.Xml.XPath;

namespace App
{
	internal sealed partial class Program
	{
		#region Fields

		private const string _lastFile = "Last";

		private const string _mainFeed = "http://www.crunchyroll.com/rss";

		private const string _altFeed = "http://feeds.feedburner.com/crunchyroll/rss/anime";

		#endregion

		private static void Main()
		{
			if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length >= 2)
			{
				Environment.Exit(0);
			}

			CancellationTokenSource cancellationTokenSource = new();

			CancellationToken cancellationToken = cancellationTokenSource.Token;

			Run(cancellationToken);

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			#region Tray Icon

			ContextMenuStrip strip = new();

			ToolStripMenuItem settingsItem = new("Settings")
			{
				Name = "Settings"
			};

			Settings? settings = null;

			settingsItem.Click += async (_, _) =>
			{
				if (settings is null)
				{
					settings = new((await ConfigManager.LoadAsync(cancellationToken)).Item2, cancellationToken);

					settings.FormClosed += (_, _) => settings = null;

					settings.Show();
				}
				else
				{
					settings.Focus();
				}
			};

			strip.Items.Add(settingsItem);

			ToolStripMenuItem configItem = new("Config")
			{
				Name = "Config"
			};

			configItem.Click += async (_, _) => await ConfigManager.OpenConfigAsync(cancellationToken);

			strip.Items.Add(configItem);

			ToolStripMenuItem exit = new("Exit")
			{
				Name = "Exit"
			};

			exit.Click += (_, _) =>
			{
				settings?.Close();

				cancellationTokenSource.Cancel();
			};

			strip.Items.Add(exit);

			using NotifyIcon icon = new()
			{
				ContextMenuStrip = strip,
				Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
				Text = Application.ProductName,
				Visible = true
			};

			#endregion

			Application.Run();

			icon.Visible = false;
		}

		#region Background Thread

		private static bool CheckValue(string? value, IEnumerable<string> values)
		{
			return !values.Any() || values.Any(v => v.Equals(value, StringComparison.OrdinalIgnoreCase));
		}

		[GeneratedRegex("\\(([A-Za-z\\-]+) Dub\\)")]
		private static partial Regex DubRegex();

		private static async void Run(CancellationToken cancellationToken)
		{
			NotificationHelper notification = new(cancellationToken);

			#region Load Config

			(LoadStatus status, FeedConfig config) = await ConfigManager.LoadAsync(cancellationToken);

			PeriodicTimer timer = new(config.Interval);

			ConfigManager.OnFeedConfigChanged += c =>
			{
				config = c;

				timer.Period = c.Interval;
			};

			#endregion

			#region Handle config file errors

			if (status != LoadStatus.Loaded)
			{
				notification.Notify($"Config was {(status is LoadStatus.NotFound ? "Not Found" : status)}", "Using Default Config", "View Defualt Config", NotificationHelper.openKey, NotificationHelper.defaultConfigValue);
			}

			#endregion

			#region Load Last Check

			DateTime last;

			if (File.Exists(_lastFile))
			{
				last = DateTime.TryParse(await File.ReadAllTextAsync(_lastFile, cancellationToken), out DateTime result) ? result : DateTime.Now;
			}
			else
			{
				last = DateTime.Now;

				try
				{
					await File.WriteAllTextAsync(_lastFile, last.ToString(), cancellationToken);
				}
				catch { }

				if (config.ShowFirstRun)
				{
					notification.Notify("App First Run", "You can find the app in the system tray.", "Open Config", NotificationHelper.openKey, NotificationHelper.openKey);
				}
			}

			#endregion

			#region Run Main Loop

			try
			{
				XmlNamespaceManager? manager = null;

				using HttpClientHandler handler = new()
				{
					UseCookies = true
				};

				do
				{
					try
					{
						#region Load RSS Feed

						using HttpClient client = new(handler, false);

						using XmlReader reader = XmlReader.Create((await client.SendAsync(new()
						{
							Method = HttpMethod.Get,
							RequestUri = new(config.FeedHost switch
							{
								FeedHostType.Crunchyroll => _mainFeed,
								FeedHostType.FeedBurner => _altFeed,
								_ => throw new NotImplementedException()
							}),
							Headers =
							{
								{ "cookie", "session_id=fb5e80d995a90f27726e04f6fb23299f" },
								{ "User-Agent", "chrome" }
							}
						}, cancellationToken)).EnsureSuccessStatusCode().Content.ReadAsStream(cancellationToken));

						if (manager is null)
						{
							manager = new(reader.NameTable);

							manager.AddNamespace("media", "http://search.yahoo.com/mrss/");
							manager.AddNamespace("crunchyroll", _mainFeed);
						}

						#endregion

						DateTime copy = last;

						#region Loop through parsed episode data

						XPathNavigator navigator = new XPathDocument(reader).CreateNavigator();

						string pub = config.Visibility is Visibility.Default ? "pub" : $"crunchyroll:{config.Visibility.ToString().ToLower()}Pub";

						foreach (XPathNavigator nav in navigator.Select($"//item[position() <= {config.MaxNotifications}]").OfType<XPathNavigator>().Reverse())
						{
							if (nav.SelectSingleNode(".//category", manager)?.Value == "Anime" && DateTime.TryParse(nav.SelectSingleNode($".//{pub}Date", manager)?.Value, out DateTime result) && result > copy)
							{
								string? dub = nav.SelectSingleNode(".//title", manager)?.Value is string title ? DubRegex().Match(title).Groups.Values.ElementAtOrDefault(1)?.Value : null;

								if (CheckValue(dub, config.Dubs) && CheckValue(nav.SelectSingleNode(".//crunchyroll:seriesTitle", manager)?.Value, config.Names) && nav.SelectSingleNode(".//link")?.Value is string url)
								{
									notification.Notify($"{nav.SelectSingleNode(".//crunchyroll:seriesTitle", manager)?.Value}{(dub is null ? string.Empty : $" ({dub})")}", $"Episode {nav.SelectSingleNode(".//crunchyroll:episodeNumber", manager)?.ValueAsInt}", NotificationHelper.openKey, url);
								}

								last = result;
							}
						}

						#endregion

						if (copy != last)
						{
							await File.WriteAllTextAsync(_lastFile, last.ToString(), cancellationToken);
						}
					}
					catch { }
				} while (await timer.WaitForNextTickAsync(cancellationToken));
			}
			catch (Exception ex)
			{
				if (ex is OperationCanceledException)
				{
					Application.Exit();
				}
			}

			#endregion
		}

		#endregion
	}
}
