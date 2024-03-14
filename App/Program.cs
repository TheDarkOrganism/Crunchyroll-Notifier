using App;
using System.Xml;
using System.Xml.XPath;

if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length >= 2)
{
	Environment.Exit(0);
}

CancellationTokenSource source = new();

CancellationToken token = source.Token;

NotificationHelper notification = new(token);

#region Background Thread

async void Run()
{

	#region Load Config

	FeedConfig config;

	PeriodicTimer timer;

	(LoadStatus, FeedConfig) load = await ConfigManager.LoadAsync(token);

	config = load.Item2;

	#endregion

	#region Handle config file errors

	LoadStatus status = load.Item1;

	if (status != LoadStatus.Loaded)
	{
		notification.Notify($"Config was {(status is LoadStatus.NotFound ? "Not Found" : status)}", "Using Default Config", "View Defualt Config", NotificationHelper.openKey, NotificationHelper.defaultConfigValue);
	}

	#endregion

	#region Load Last Check

	const string lastFile = "Last";

	DateTime last;

	if (File.Exists(lastFile))
	{
		last = DateTime.TryParse(await File.ReadAllTextAsync(lastFile, token), out DateTime result) ? result : DateTime.Now;
	}
	else
	{
		last = DateTime.Now;

		try
		{
			await File.WriteAllTextAsync(lastFile, last.ToString(), token);
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
		static bool CheckValue(string? value, IEnumerable<string> values)
		{
			return !values.Any() || values.Any(v => v.Equals(value, StringComparison.OrdinalIgnoreCase));
		}

		timer = new(TimeSpan.FromSeconds(config.Interval));

		ConfigManager.OnFeedConfigChanged += c =>
		{
			config = c;

			timer.Period = TimeSpan.FromSeconds(c.Interval);
		};

		XmlNamespaceManager? manager = default;

		do
		{
			try
			{
				#region Load RSS Feed

				XPathDocument document = new("http://feeds.feedburner.com/crunchyroll/rss/anime");

				XPathNavigator navigator = document.CreateNavigator();

				if (manager is null)
				{
					manager = new(navigator.NameTable);

					manager.AddNamespace("media", "http://search.yahoo.com/mrss/");
					manager.AddNamespace("crunchyroll", "http://www.crunchyroll.com/rss");
				}

				#endregion

				DateTime copy = last;

				#region Loop through parsed episode data

				string pub = config.Visibility is Visibility.Default ? "pub" : $"crunchyroll:{config.Visibility.ToString().ToLower()}Pub";

				foreach (XPathNavigator nav in navigator.Select($"//item[position() <= {config.MaxNotifications}]").OfType<XPathNavigator>().Reverse())
				{
					if (DateTime.TryParse(nav.SelectSingleNode($".//{pub}Date", manager)?.Value, out DateTime result) && result > copy)
					{
						string? dub = nav.SelectSingleNode(".//title", manager)?.Value is string title ? Regex.Match(title, @"\(([A-Za-z\-]+) Dub\)").Groups.Values.ElementAtOrDefault(1)?.Value : default;

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
					await File.WriteAllTextAsync(lastFile, last.ToString(), token);
				}
			}
			catch { }
		} while (await timer.WaitForNextTickAsync(token));
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

Run();

#endregion

Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);

#region Tray Icon

ContextMenuStrip strip = new();

ToolStripMenuItem settingsItem = new("Settings")
{
	Name = "Settings"
};

Settings? settings = default;

settingsItem.Click += async (_, _) =>
{
	if (settings is null)
	{
		settings = new((await ConfigManager.LoadAsync(token)).Item2, token);

		settings.FormClosed += (_, _) => settings = default;

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

configItem.Click += async (_, _) => await ConfigManager.OpenConfigAsync(token);

strip.Items.Add(configItem);

ToolStripMenuItem exit = new("Exit")
{
	Name = "Exit"
};

exit.Click += (_, _) =>
{
	settings?.Close();

	source.Cancel();
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