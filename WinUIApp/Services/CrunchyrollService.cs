using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace WinUIApp.Services
{
	internal sealed partial class CrunchyrollService(ISavableJsonOptions<IConfigModel> configOptions, ISavableJsonOptions<ILastUpdateModel> lastOptions, IHttpClientFactory httpClientFactory, NotificationHelper notificationHelper, ILogger<CrunchyrollService> logger, ILogger<FeedSource> feedSourceLogger) : IHostedService
	{
		private static readonly XmlReaderSettings _xmlReaderSettings = new()
		{
			DtdProcessing = DtdProcessing.Ignore
		};

		private readonly Dictionary<FeedHostType, FeedSource> _feedSources = new()
		{
			{ FeedHostType.Crunchyroll, new("https://www.crunchyroll.com/rss/anime", feedSourceLogger) },
			{ FeedHostType.FeedBurner, new("https://feeds.feedburner.com/crunchyroll/rss/anime", feedSourceLogger) }
		};

		private static bool CheckValue(string? value, ObservableCollection<string> values)
		{
			return string.IsNullOrWhiteSpace(value) || values.Count == 0 || values.Contains(value, StringComparer.CurrentCultureIgnoreCase);
		}

		[GeneratedRegex("\\(([A-Za-z\\-]+) Dub\\)")]
		private static partial Regex DubRegex();

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			#region Load Config

			IConfigModel configModel = configOptions.Value;

			using PeriodicTimer periodicTimer = new(configModel.Interval);

			configModel.PropertyChanged += (_, args) =>
			{
				if (args.PropertyName == nameof(ConfigModel.Interval))
				{
					periodicTimer.Period = configModel.Interval;
				}
			};

			#endregion

			ILastUpdateModel lastUpdateModel = lastOptions.Value;

			#region Run Main Loop

			try
			{
				XmlNamespaceManager? manager = null;

				do
				{
					FeedSource? source = null;

					HttpResponseMessage? httpResponse = null;

					try
					{
						#region Load RSS Feed

						using HttpClient httpClient = httpClientFactory.CreateClient();

						foreach (KeyValuePair<FeedHostType, FeedSource> pair in _feedSources.OrderBy(pair => pair.Key != configModel.FeedHost))
						{
							source = pair.Value;

							if (source.GetSource() is Uri uri)
							{
								try
								{
									httpResponse = await httpClient.GetAsync(uri, cancellationToken);

									if (source.ValidateResponse(httpResponse))
									{
										break;
									}
									else
									{
										continue;
									}
								}
								catch (HttpRequestException ex)
								{
									logger.LogDebug(ex, "Unable to load the feed for {HostType}.", pair.Key);

									continue;
								}
							}
						}

						if (source is null || !source.ValidateResponse(httpResponse))
						{
							logger.LogWarning("Unable to load the any RSS feeds.");

							continue;
						}

						using Stream stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);

						using XmlReader reader = XmlReader.Create(stream, _xmlReaderSettings);

						if (manager is null)
						{
							manager = new(reader.NameTable);

							manager.AddNamespace("media", "http://search.yahoo.com/rss/");
							manager.AddNamespace("crunchyroll", "http://www.crunchyroll.com/rss");
						}

						#endregion

						DateTime copy = lastUpdateModel.LastUpdate;

						#region #region Loop through parsed episode data

						XPathNavigator navigator = new XPathDocument(reader).CreateNavigator();

						string pub = configModel.Visibility is VisibilityType.Default ? "pub" : $"crunchyroll:{configModel.Visibility.ToString().ToLower(CultureInfo.InvariantCulture)}Pub";

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
						logger.LogError(ex, "Unable to read the RSS feed.");
					}
					finally
					{
						httpResponse?.Dispose();

						httpResponse = null;
					}
				} while (await periodicTimer.WaitForNextTickAsync(cancellationToken));
			}
			catch (Exception ex)
			{
				if (ex is OperationCanceledException)
				{
					logger.LogDebug("Main loop stopped.");
				}
				else
				{
					logger.LogError(ex, "The main loop had a unknown error.");
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
