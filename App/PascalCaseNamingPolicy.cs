namespace App
{
	internal sealed class PascalCaseNamingPolicy : JsonNamingPolicy
	{
		public override string ConvertName(string name)
		{
			if (string.IsNullOrWhiteSpace(name) || char.IsUpper(name[0]))
			{
				return name;
			}

			return string.Create(name.Length, CamelCase.ConvertName(name), (Span<char> chars, string name) =>
			{
				name.CopyTo(chars);
				chars[0] = char.ToUpper(chars[0]);
			});
		}
	}
}
