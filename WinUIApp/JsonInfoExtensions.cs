namespace WinUIApp
{
	internal static class JsonInfoExtensions
	{
		public static bool TryGetJsonPropertyInfo(this JsonTypeInfo jsonTypeInfo, string name, [NotNullWhen(true)] out JsonPropertyInfo? propertyInfo)
		{
			ArgumentNullException.ThrowIfNull(jsonTypeInfo, nameof(jsonTypeInfo));
			ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));

			propertyInfo = jsonTypeInfo.Properties.FirstOrDefault(property => property.Name == name);

			return propertyInfo is not null;
		}

		public static bool TryGetAttributes<TAttribute>(this JsonPropertyInfo jsonPropertyInfo, [NotNullWhen(true)] out IEnumerable<TAttribute>? attributes)
			where TAttribute : notnull, Attribute
		{
			ArgumentNullException.ThrowIfNull(jsonPropertyInfo, nameof(jsonPropertyInfo));

			attributes = jsonPropertyInfo.AttributeProvider?.GetCustomAttributes(typeof(TAttribute), false).OfType<TAttribute>();

			return attributes is not null;
		}
	}
}
