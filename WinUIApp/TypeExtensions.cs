namespace WinUIApp
{
	internal static class TypeExtensions
	{
		private static readonly JsonKeyComparer _keyComparer = new();

		public static IEnumerable<JsonPropertyInfo> GetDeclaredProperties(this Type type)
		{
			return ModelSerializerContext.Default.GetTypeInfo(type)?.Properties
				.Where(property => property.DeclaringType == type) ?? [];
		}

		public static Dictionary<string, ValidationAttribute[]> GetValidationAttributes(this Type type)
		{
			return GetDeclaredProperties(type)
				.Where(static property => property.AttributeProvider?.IsDefined(typeof(ValidationAttribute), true) is true && !property.AttributeProvider.IsDefined(typeof(JsonIgnoreAttribute), true))
				.ToDictionary(static property => property.Name, static property => (property.AttributeProvider?.GetCustomAttributes(true).OfType<ValidationAttribute>() ?? []).ToArray(), _keyComparer);
		}
	}
}
