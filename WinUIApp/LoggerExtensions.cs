namespace WinUIApp
{
	internal static partial class LoggerExtensions
	{
		[LoggerMessage(LogLevel.Debug, "The operation was canceled.")]
		public static partial void LogOperationCanceled(this ILogger logger, OperationCanceledException ex);

		#region Models.FileModel

		[LoggerMessage(LogLevel.Debug, "The {Type} object was disposed.")]
		public static partial void LogObjectDisposed(this ILogger logger, ObjectDisposedException ex, Type type);

		[LoggerMessage(LogLevel.Error, "Failed to wait for the file {File}.")]
		public static partial void LogWaitFailed(this ILogger logger, Exception exception, string file);

		#endregion

		#region Options.SavableJsonOptions

		[LoggerMessage(LogLevel.Debug, "{File} can't be saved due to the {Model} not being modified.")]
		public static partial void LogNotModified(this ILogger logger, string file, Type model);

		[LoggerMessage(LogLevel.Debug, "{File} can't be saved due to the {Model} having validation errors.")]
		public static partial void LogHasValidationErrors(this ILogger logger, string file, Type model);

		[LoggerMessage(LogLevel.Error, "Failed to get FileStream for {File}.")]
		public static partial void LogFailGetFileStream(this ILogger logger, Exception exception, string file);

		[LoggerMessage(LogLevel.Warning, "The configuration for {File} is not a valid JSON object.")]
		public static partial void LogInvalidJsonConfigiurationObject(this ILogger logger, string file);

		[LoggerMessage(LogLevel.Warning, "The configuration for {File} is not a valid JSON document.")]
		public static partial void LogInvalidJsonConfigiurationDocument(this ILogger logger, Exception ex, string file);

		[LoggerMessage(LogLevel.Error, "Failed to serialize FileStream for {File}")]
		public static partial void LogFailedFileStreamSerialization(this ILogger logger, Exception ex, string file);

		#endregion

		[LoggerMessage(LogLevel.Error, "Unable to read JSON {File}.")]
		public static partial void LogFailedFileRead(this ILogger logger, Exception ex, string? file);

		#region Providers.JsonProviderBase

		[LoggerMessage(LogLevel.Warning, "Failed to load JSON from {File}.")]
		public static partial void LogFailedJsonConfigurationLoad(this ILogger logger, JsonException ex, string? file);

		#endregion

		[LoggerMessage(LogLevel.Debug, "The current JSON token is not a {JsonTokenType}.")]
		public static partial void LogInvalidJsonTokenType(this ILogger logger, JsonTokenType jsonTokenType);

		#region Providers.ValidationConfigurationProvider

		[LoggerMessage(LogLevel.Debug, "Unable to desrialize the JSON token {JsonTokenType} as {Type}.")]
		public static partial void LogFailedTokenDeserialization(this ILogger logger, JsonException ex, JsonTokenType jsonTokenType, Type type);

		[LoggerMessage(LogLevel.Warning, "Unable to desrialize the JSON token {JsonTokenType}.")]
		public static partial void LogFailedTokenDeserialization(this ILogger logger, Exception ex, JsonTokenType jsonTokenType);

		[LoggerMessage(LogLevel.Warning, "Unable to parse JSON token {JsonTokenType} as {Type}.")]
		public static partial void LogFailedToParseToken(this ILogger logger, JsonTokenType jsonTokenType, Type type);

		[LoggerMessage(LogLevel.Trace, "Parsing finished for {File}.")]
		public static partial void LogParsingFinished(this ILogger logger, string? file);

		[LoggerMessage(LogLevel.Trace, "Parsed property {PropertyName}.")]
		public static partial void LogParsedProperty(this ILogger logger, string propertyName);

		[LoggerMessage(LogLevel.Trace, "Skipped property {PropertyName}.")]
		public static partial void LogSkippedProperty(this ILogger logger, string propertyName);

		[LoggerMessage(LogLevel.Warning, "Failed to validate {Attribute} with {Type} for {PropertyName}.")]
		public static partial void LogInvalidProperty(this ILogger logger, ValidationAttribute attribute, Type type, string propertyName);

		[LoggerMessage(LogLevel.Warning, "The {Attribute} with {Value} must be between {Min} and {Max} for {PropertyName}.")]
		public static partial void LogInvalidPropertyRange(this ILogger logger, ValidationAttribute attribute, object value, object min, object max, string propertyName);

		[LoggerMessage(LogLevel.Trace, "Parsing JSON configuration from {File}.")]
		public static partial void LogParsingConfiguration(this ILogger logger, string? file);

		#endregion

		#region Services.CrunchyrollService

		[LoggerMessage(LogLevel.Debug, "Unable to load the feed for {FeedHostType}.")]
		public static partial void LogFailedFeedHostLoad(this ILogger logger, HttpRequestException ex, FeedHostType feedHostType);

		[LoggerMessage(LogLevel.Warning, "Unable to load the any RSS feeds.")]
		public static partial void LogNoRSSFeeds(this ILogger logger);

		[LoggerMessage(LogLevel.Error, "The RSS feed is not valid XML.")]
		public static partial void LogInvalidRSS(this ILogger logger, XmlException ex);

		[LoggerMessage(LogLevel.Error, "Unable to read the RSS feed.")]
		public static partial void LogInvalidFeed(this ILogger logger, Exception ex);

		[LoggerMessage(LogLevel.Debug, "Main loop stopped.")]
		public static partial void LogMainLoopStopped(this ILogger logger);

		[LoggerMessage(LogLevel.Error, "The main loop had a unknown error.")]
		public static partial void LogFailedMainLoop(this ILogger logger, Exception ex);

		#endregion

		#region Services.FeedSource

		[LoggerMessage(LogLevel.Warning, "The request to {Source} must be after {Delay}.")]
		public static partial void LogEarlyRequest(this ILogger logger, Uri source, DateTime delay);

		[LoggerMessage(LogLevel.Debug, "{RequestUri} was not {Source}.")]
		public static partial void LogUriMismatch(this ILogger logger, Uri? requestUri, Uri source);

		[LoggerMessage(LogLevel.Warning, "The request to {Source} was rate-limited until {Delay}.")]
		public static partial void LogRateLimited(this ILogger logger, Uri source, DateTime delay);

		[LoggerMessage(LogLevel.Error, "The Content-Type was null or empty for request to {Source}.")]
		public static partial void LogNullContentType(this ILogger logger, Uri source);

		[LoggerMessage(LogLevel.Warning, "The Content-Type for the {Source} was an invalid {ContentType} and should be {Xml} or {RSS}.")]
		public static partial void LogInvalidContentType(this ILogger logger, Uri source, string contentType, string xml, string rss);

		#endregion
	}
}
