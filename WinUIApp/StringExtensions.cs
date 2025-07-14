namespace WinUIApp
{
	internal static class StringExtensions
	{
		private static bool ShouldAddSpaces(string input, int index, char c)
		{
			return index > 0 && !char.IsWhiteSpace(input[index - 1]) && char.IsUpper(c);
		}

		public static string AddSpaces(this string input)
		{
			ArgumentNullException.ThrowIfNull(input, nameof(input));

			int length = input.Length;

			int newLength = length + input.Where((c, i) => ShouldAddSpaces(input, i, c)).Count();

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

					if (ShouldAddSpaces(state, i, c))
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
