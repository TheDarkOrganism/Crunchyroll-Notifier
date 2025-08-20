using System.Net;

namespace WinUIApp.Services
{
	internal sealed class FeedSource
	{
		private DateTime _delay = DateTime.MinValue;

		private int _backOff;

		private readonly Uri _source;

		private readonly ILogger<FeedSource> _logger;

		public FeedSource([StringSyntax(StringSyntaxAttribute.Uri)] string url, ILogger<FeedSource> logger)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(url, nameof(url));

			_source = Uri.TryCreate(url, UriKind.Absolute, out Uri? result) ? result : throw new ArgumentException("Invalid URL format.", nameof(url));

			_logger = logger;
		}

		public Uri? GetSource()
		{
			if (_delay > DateTime.Now || _backOff > 0)
			{
				_logger.LogWarning("The request to {Source} must be after {Delay}.", _source, _delay);

				return null;
			}

			return _source;
		}

		public bool ValidateResponse([NotNullWhen(true)] HttpResponseMessage? responseMessage)
		{
			if (responseMessage is null)
			{
				return false;
			}

			if (responseMessage.RequestMessage?.RequestUri?.AbsoluteUri != _source.AbsoluteUri)
			{
				_logger.LogDebug("The {RequestMessage}.{UriName} was not {Source}.", nameof(HttpRequestMessage), nameof(HttpRequestMessage.RequestUri), _source);

				return false;
			}

			if (!responseMessage.IsSuccessStatusCode)
			{
				if (responseMessage.StatusCode == HttpStatusCode.TooManyRequests)
				{
					if (responseMessage.Headers.TryGetValues("Retry-After", out IEnumerable<string>? strings) && int.TryParse(strings.FirstOrDefault(), out int retry))
					{
						_delay = DateTime.Now.AddSeconds(retry);
					}
					else
					{
						_delay = DateTime.Now.AddSeconds(Math.Pow(2, _backOff));

						_backOff++;
					}

					_logger.LogWarning("The request to {Source} was Rate-limited until {Delay}.", _source, _delay);

					return false;
				}
			}

			string? contentType = responseMessage.Content.Headers.ContentType?.MediaType;

			if (string.IsNullOrWhiteSpace(contentType))
			{
				_logger.LogError("The Content-Type was null or empty for request to {Source}.", _source);

				return false;
			}

			if (contentType is "text/xml" or "application/xml+rss")
			{
				_delay = DateTime.MinValue;

				_backOff = 0;

				return true;
			}

			_logger.LogWarning("The Content-Type for the {Source} was an invalid {ContentType}.", _source, contentType);

			return false;
		}
	}
}
