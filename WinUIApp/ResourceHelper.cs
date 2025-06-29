namespace WinUIApp
{
	internal static class ResourceHelper
	{
		private static readonly EmbeddedFileProvider _embeddedFileProvider = new(Assembly.GetExecutingAssembly(), nameof(WinUIApp));

		public static void RestoreFile(string file)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(file, nameof(file));

			IFileInfo fileInfo = _embeddedFileProvider.GetFileInfo(Path.GetFileName(file));

			if (!fileInfo.Exists)
			{
				throw new FileNotFoundException("Unable to find the file as a embedded resource.", file);
			}

			using Stream stream = fileInfo.CreateReadStream();

			using FileStream fileStream = File.Create(file);

			fileStream.SetLength(0);

			stream.CopyTo(fileStream);
		}
	}
}
