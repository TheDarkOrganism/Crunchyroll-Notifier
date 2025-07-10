
namespace WinUIApp
{
	internal static class TypeExtensions
	{
		private static bool PropertyFilter(PropertyInfo propertyInfo)
		{
			return propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>() is null;
		}

		private static ValidationAttribute[] GetValidationAttributes(PropertyInfo propertyInfo)
		{
			return [.. propertyInfo.GetCustomAttributes<ValidationAttribute>()];
		}

		public static Dictionary<string, ValidationAttribute[]> GetValidationAttributes([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] this Type type)
		{
			return type.GetProperties().Where(PropertyFilter).ToDictionary(static property => property.Name, GetValidationAttributes);
		}

		public static Dictionary<string, ValidationAttribute[]> GetValidationAttributes([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] this Type type, Func<PropertyInfo, string> keyResolver)
		{
			return type.GetProperties().Where(PropertyFilter).ToDictionary(keyResolver, GetValidationAttributes);
		}
	}
}
