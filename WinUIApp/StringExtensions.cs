namespace WinUIApp
{
	internal static class StringExtensions
	{
		public static string AddSpaces(this string input)
		{
			ArgumentNullException.ThrowIfNull(input, nameof(input));

			int length = input.Length;

			int newLength = length + input.Skip(1).Count(char.IsUpper);

			if (length == newLength)
			{
				return input;
			}

			return string.Create(newLength, input, static (span, state) =>
			{
				int offset = 0;

				for (int i = 0; i < state.Length; i++)
				{
					char c = state[i];

					if (i > 0 && char.IsUpper(c))
					{
						span[i + offset] = ' ';

						offset++;
					}

					span[i + offset] = c;
				}
			});
		}
	}
}
