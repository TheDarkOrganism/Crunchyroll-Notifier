namespace WinUIApp.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal sealed class LanguageAttribute : ValidationAttribute
	{
		private static readonly StringComparer _stringComparer = StringComparer.OrdinalIgnoreCase;

		private static readonly HashSet<string> _names = CultureInfo.GetCultures(CultureTypes.AllCultures).Skip(1).Select(static culture => culture.DisplayName.Split(' ')[0]).Distinct(_stringComparer).ToHashSet(_stringComparer);

		public override string FormatErrorMessage(string name)
		{
			return string.Format("The language {0} is invalid.", name);
		}

		public override bool IsValid(object? value)
		{
			if (value is not string str || string.IsNullOrWhiteSpace(str))
			{
				return false;
			}

			if (_names.Contains(str))
			{
				return true;
			}

			int index = str.LastIndexOf('-');

			return index > -1 && _names.Contains(str[..index]) && _names.Contains(str[(index + 1)..]);
		}
	}
}
