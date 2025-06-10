using System.Collections;

namespace WinUIApp.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal sealed class NotEmptyAttribute : ValidationAttribute
	{
		public override bool IsValid(object? value)
		{
			return value switch
			{
				string str => !string.IsNullOrWhiteSpace(str),
				ICollection values => values.Count > 0,
				IEnumerable objects => objects.OfType<object>().Any(),
				_ => base.IsValid(value),
			};
		}

		public override string FormatErrorMessage(string name)
		{
			return string.Format("The field {0} was empty.", name);
		}
	}
}
