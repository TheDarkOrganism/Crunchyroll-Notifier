using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace WinUIApp.Services
{
	internal sealed partial class CrunchyrollService(ISavableJsonOptions<ConfigModel> configOptions, ISavableJsonOptions<LastUpdateModel> lastOptions, IHttpClientFactory httpClientFactory, NotificationHelper notificationHelper) : IHostedService
	{
		private static bool CheckValue(string? value, ObservableCollection<string> values)
		{
			return string.IsNullOrWhiteSpace(value) || values.Count == 0 || values.Contains(value, StringComparer.OrdinalIgnoreCase);
		}

		[GeneratedRegex("\\(([A-Za-z\\-]+) Dub\\)")]
		private static partial Regex DubRegex();

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			#region Load Config

			ConfigModel configModel = configOptions.Value;

			using PeriodicTimer periodicTimer = new(configModel.Interval);

			configModel.PropertyChanged += (_, args) =>
			{
				if (args.PropertyName == nameof(ConfigModel.Interval))
				{
					periodicTimer.Period = configModel.Interval;
				}
			};

			#endregion

			LastUpdateModel lastUpdateModel = lastOptions.Value;

			#region Run Main Loop

			try
			{
				XmlNamespaceManager? manager = null;

				do
				{
					try
					{
						#region Load RSS Feed

						using HttpClient httpClient = httpClientFactory.CreateClient();

						using XmlReader reader = XmlReader.Create(await httpClient.GetStreamAsync(configModel.FeedHost switch
						{
							FeedHostType.Crunchyroll => "http://www.crunchyroll.com/rss/anime",
							FeedHostType.FeedBurner => "http://feeds.feedburner.com/crunchyroll/rss/anime",
							_ => throw new NotImplementedException()
						}, cancellationToken));

						if (manager is null)
						{
							manager = new(reader.NameTable);

							manager.AddNamespace("media", "http://search.yahoo.com/rss/");
							manager.AddNamespace("crunchyroll", "http://www.crunchyroll.com/rss");
						}

						#endregion

						DateTime copy = lastOptions.Value.LastUpdate;

						#region #region Loop through parsed episode data

						XPathNavigator navigator = new XPathDocument(reader).CreateNavigator();

						string pub = configModel.Visibility is VisibilityType.Default ? "pub" : $"crunchyroll:{configModel.Visibility.ToString().ToLower(CultureInfo.CurrentCulture)}Pub";

						foreach (XPathNavigator nav in navigator.Select($"//item[position() <= {configModel.MaxNotifications}]").OfType<XPathNavigator>().Reverse())
						{
							if (nav.SelectSingleNode(".//category", manager)?.Value == "Anime" && DateTime.TryParse(nav.SelectSingleNode($".//{pub}Date", manager)?.Value, out DateTime result) && result > copy)
							{
								string? dub = nav.SelectSingleNode(".//title", manager)?.Value is string title ? DubRegex().Match(title).Groups.Values.ElementAtOrDefault(1)?.Value : null;

								if (CheckValue(dub, configModel.Dubs) && CheckValue(nav.SelectSingleNode(".//crunchyroll:seriesTitle", manager)?.Value, configModel.Names) && Uri.TryCreate(nav.SelectSingleNode(".//link")?.Value, UriKind.Absolute, out Uri? uri))
								{
									notificationHelper.Notify($"{nav.SelectSingleNode(".//crunchyroll:seriesTitle", manager)?.Value}{(dub is null ? string.Empty : $" ({dub})")}", $"Episode {nav.SelectSingleNode(".//crunchyroll:episodeNumber", manager)?.ValueAsInt}", "Open", uri);
								}

								lastUpdateModel.LastUpdate = result;
							}
						}

						if (lastUpdateModel.LastUpdate != copy)
						{
							await lastOptions.SaveAsync();
						}

						#endregion
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex);
					}
				} while (await periodicTimer.WaitForNextTickAsync(cancellationToken));
			}
			catch (Exception ex)
			{
				if (ex is OperationCanceledException)
				{
					Application.Current.Exit();
				}
				else
				{
					Debug.WriteLine(ex);
				}
			}

			#endregion
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			await lastOptions.SaveAsync();
		}
	}
}
