namespace WinUIApp
{
	internal static class TypeExtensions
	{
		private static readonly JsonKeyComparer _keyComparer = new();

		private static bool PropertyFilter(JsonPropertyInfo propertyInfo)
		{
			return propertyInfo.AttributeProvider?.IsDefined(typeof(JsonIgnoreAttribute), true) is false;
		}

		public static ValidationAttribute[] GetValidationAttributes(this JsonPropertyInfo propertyInfo)
		{
			return [.. propertyInfo.AttributeProvider?.GetCustomAttributes(true).OfType<ValidationAttribute>() ?? []];
		}

		public static IEnumerable<JsonPropertyInfo> GetDeclaredProperties(this Type type)
		{
			JsonTypeInfo? jsonTypeInfo = ModelSerializerContext.Default.GetTypeInfo(type);

			return jsonTypeInfo?.Properties.Where(property => property.DeclaringType == type) ?? [];
		}

		public static Dictionary<string, ValidationAttribute[]> GetValidationAttributes(this Type type)
		{
			return GetDeclaredProperties(type).Where(PropertyFilter).ToDictionary(static property => property.Name, GetValidationAttributes, _keyComparer);
		}
	}
}
