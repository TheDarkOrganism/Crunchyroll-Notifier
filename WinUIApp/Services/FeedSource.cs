using System.Net;

namespace WinUIApp.Services
{
	internal sealed class FeedSource
	{
		private const string _xmlContentType = "text/xml";

		private const string _rssContentType = "application/xml+rss";

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
				_logger.LogEarlyRequest(_source, _delay);

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

			Uri? responseUri = responseMessage.RequestMessage?.RequestUri;

			if (responseUri?.AbsoluteUri != _source.AbsoluteUri)
			{
				_logger.LogUriMismatch(responseUri, _source);

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

					_logger.LogRateLimited(_source, _delay);

					return false;
				}
			}

			string? contentType = responseMessage.Content.Headers.ContentType?.MediaType;

			if (string.IsNullOrWhiteSpace(contentType))
			{
				_logger.LogNullContentType(_source);

				return false;
			}

			if (contentType is _xmlContentType or _rssContentType)
			{
				_delay = DateTime.MinValue;

				_backOff = 0;

				return true;
			}

			_logger.LogInvalidContentType(_source, contentType, _xmlContentType, _rssContentType);

			return false;
		}
	}
}
