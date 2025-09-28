namespace WinUIApp
{
	internal static class ResourceHelper
	{
		private static readonly EmbeddedFileProvider _embeddedFileProvider = new(Assembly.GetExecutingAssembly(), nameof(WinUIApp));

		public static IFileInfo GetResource(string file)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));

			IFileInfo fileInfo = _embeddedFileProvider.GetFileInfo(Path.GetFileName(file));

			if (!fileInfo.Exists)
			{
				throw new FileNotFoundException("Unable to find the file as a embedded resource.", file);
			}

			return fileInfo;
		}
	}
}
