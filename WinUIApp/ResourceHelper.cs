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

		public static void RestoreFile<TIModel, T>(IFileModel<TIModel> fileModel, ILogger<T> logger)
			where TIModel : class, IModelBase
		{
			ArgumentNullException.ThrowIfNull(fileModel, nameof(fileModel));

			fileModel.Wait(() =>
			{
				try
				{
					string file = fileModel.File;

					using Stream stream = GetResource(file).CreateReadStream();

					using FileStream fileStream = File.Create(file);

					fileStream.SetLength(0);

					stream.CopyTo(fileStream);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);

					Application.Current.Exit();
				}
			}, logger);
		}
	}
}
